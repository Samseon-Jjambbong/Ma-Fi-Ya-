
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;
using UnityEngine.UI;

public class MafiaGameFlow : MonoBehaviourPun
{
    [Header("")]
    [SerializeField] private GameTimer timer;
    [SerializeField] private LightController lightController;
    [SerializeField] private RoleUI roleUI;
    [SerializeField] private Button skipVoteButton;
    [SerializeField] private WinLoseUI winLoseUI;
    [SerializeField] string MenuSceneName;
    [SerializeField] const int WINPOINTS = 100;
    [SerializeField] const int LOSEPOINTS = 50;
    private Coroutine timerRoutine;

    [Header("System Message")]
    [SerializeField] Color MSGColor = new Color(0.372549f, 0.3647059f, 0.6117647f);
    [Range(0,1)]
    [SerializeField] float MSGAlpha = 1f;
    [SerializeField] const string  VOTESTART = "Voting started";
    [SerializeField] const string VOTEFINISH = "Voting finished";
    [SerializeField] const string NOONEDIED = "No one died last night";
    [SerializeField] const string NIGHT2DAY = "Night has come...";
    [SerializeField] const string DAY2NIGHT = "It's Day Time...";
     private ChatData chatData;
    private void Start()
    {
        Manager.Mafia.IsDay = true;
        chatData = new ChatData();
        chatData.messageColor = MSGColor;
        chatData.messageColor.a = MSGAlpha;
    }

    private void Ready()
    {
        PhotonNetwork.LocalPlayer.SetMafiaReady(true);
    }

    /******************************************************
    *                    RPCs + Methods
    ******************************************************/
    #region RPCs
    [PunRPC]
    public void DisplayRole(int time)
    {
        StartCoroutine(DisplayRoleRoutine(time));
    }

    [PunRPC]
    public void StartNightPhase(int time)
    {
        StartCoroutine(NightPhaseRoutine(time));
    }

    [PunRPC]
    public void ShowNightEvents()
    {
        StartCoroutine(ShowNightEventsRoutine());
    }

    [PunRPC]
    public void ShowNightResults()
    {
        StartCoroutine(ShowNightResultsRoutine());
    }

    [PunRPC]
    public void StartDayPhase(int time)
    {
        StartCoroutine(DayPhaseRoutine(time));
    }

    [PunRPC]
    public void ShowVoteResults(int voteResult)
    {
        StartCoroutine(ShowVoteResultsRoutine(voteResult));
    }

    [PunRPC]
    public void GameOver(int gameResult)
    {
        StartCoroutine(GameOverRoutine(gameResult));
    }

    public void ChangeTime()
    {
        StartCoroutine(ChangeTimeOfDayRoutine());
    }

    public void DisableSkipButton()
    {
        skipVoteButton.interactable = false;
    }

    public void EnableChat(bool enable)
    {
        MafiaGameChatManager.Instance.IsChatable = enable;
    }

    #endregion

    /******************************************************
    *                    Coroutines
    ******************************************************/
    #region Coroutines
    // Display Role for X seconds
    private IEnumerator DisplayRoleRoutine(int time)
    {
        roleUI.gameObject.SetActive(true);
        roleUI.InitBegin();
        yield return timer.StartTimer(time);
        roleUI.gameObject.SetActive(false);
        Ready();
    }

    // Display Kicked/Killed Player's Role for X Seconds
    public IEnumerator RemovedPlayerRoleRoutine(int playerID)
    {
        roleUI.gameObject.SetActive(true);
        roleUI.InitDead(playerID);
        yield return timer.StartTimer(3);
        roleUI.gameObject.SetActive(false);
    }

    // Day/Night Light Changer
    private IEnumerator ChangeTimeOfDayRoutine()
    {
        yield return lightController.ChangePhase();
        Manager.Mafia.IsDay = !Manager.Mafia.IsDay; //낮이면 밤으로, 밤이면 낮으로
        EnableChat(Manager.Mafia.IsDay);
    }

    // Night Phase
    private IEnumerator NightPhaseRoutine(int time)
    {
        // Day -> Night
        yield return ChangeTimeOfDayRoutine();
        chatData.message = NIGHT2DAY;
        MafiaGameChatManager.Instance.PublishMessage(chatData);

        // Prevent dead players from using skills
        if (PhotonNetwork.LocalPlayer.GetDead())
        {
            yield return new WaitForSeconds(3);
            Ready();
            yield break;
        }

        // Allow chat for mafia
        EnableChat(false);

        // Allow skill usage for X Seconds
        Manager.Mafia.ActivateHouseOutlines();

        yield return timer.StartTimer(time);

        Manager.Mafia.DeactivateHouseOutlines();

        // Store skill usage info
        Manager.Mafia.NotifyAction();

        yield return new WaitForSeconds(3); // Give time for network to receive actions

        Ready();
    }

    
    private IEnumerator ShowNightEventsRoutine()
    {
        Manager.Mafia.sharedData.photonView.RPC("ResetClientFinishedCount", RpcTarget.All);
        Manager.Mafia.photonView.RPC("ParseActionsAndAssign", RpcTarget.MasterClient);
        yield return new WaitForSeconds(1);
        Manager.Mafia.photonView.RPC("ShowActions", RpcTarget.All);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => Manager.Mafia.sharedData.clientFinishedCount == Manager.Mafia.ActivePlayerCount());
        Ready();
        Manager.Mafia.sharedData.photonView.RPC("ClearActionInfo", RpcTarget.All);
    }

    private IEnumerator ShowNightResultsRoutine()
    {
        // Night -> Day
        yield return ChangeTimeOfDayRoutine();
        chatData.message = DAY2NIGHT;
        MafiaGameChatManager.Instance.PublishMessage(chatData);

        // Show Players that died last night
        List<int> killed = Manager.Mafia.sharedData.GetKilledPlayers();
        if (killed.Count == 0)
        {
            Debug.Log(NOONEDIED);
            chatData.message = NOONEDIED;
            MafiaGameChatManager.Instance.PublishMessage(chatData);
        }
        else
        {
            // Show player die animation
            yield return Manager.Mafia.ShowKilledPlayers(killed);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        Ready();
    }

    // Allow Chat and voting for X Seconds
    private IEnumerator DayPhaseRoutine(int time)
    {
        // Prevent dead players from voting
        if (PhotonNetwork.LocalPlayer.GetDead())
        {
            yield return new WaitForSeconds(3);
            Ready();
            yield break;
        }
            
        // Allow chat for everyone
        EnableChat(true);

        // Allow voting for X Seconds
        Manager.Mafia.ShowVoteCounts();
        skipVoteButton.gameObject.SetActive(true);
        Manager.Mafia.ActivateHouseOutlines();

        Debug.Log(VOTESTART);
        chatData.message = VOTESTART;
        MafiaGameChatManager.Instance.PublishMessage(chatData);

        timerRoutine = StartCoroutine(timer.StartTimer(time));
        while (!timer.timerFinished && !(Manager.Mafia.voteCount == Manager.Mafia.sharedData.ActivePlayerCount()))
        {
            yield return new WaitForSeconds(1);
        }
        StopCoroutine(timerRoutine);
        yield return timer.StartTimer(1);
        Manager.Mafia.voteCount = 0;

        Debug.Log(VOTEFINISH);
        chatData.message = VOTEFINISH;
        MafiaGameChatManager.Instance.PublishMessage(chatData);

        Manager.Mafia.HideVoteCounts();
        skipVoteButton.gameObject.SetActive(false);
        Manager.Mafia.DeactivateHouseOutlines();

        EnableChat(false);
        Ready();
    }

    private IEnumerator ShowVoteResultsRoutine(int voteResult)
    {
        yield return Manager.Mafia.animFactory.PlayerKickedActionRoutine(voteResult);
        yield return new WaitForSeconds(1);
        yield return RemovedPlayerRoleRoutine(voteResult);
        Manager.Mafia.ApplyVoteResult(voteResult);
        yield return new WaitForSeconds(1);
        Ready();
    }

    private IEnumerator GameOverRoutine(int gameResult)
    {
        MafiaResult result = (MafiaResult) gameResult;
        // 내가 마피아면
        if (PhotonNetwork.LocalPlayer.GetPlayerRole() == MafiaRole.Mafia)
        {
            if (result == MafiaResult.MafiaWin)
            {
                winLoseUI.ShowWin(WINPOINTS);
            }
            else
            {
                winLoseUI.ShowLose(LOSEPOINTS);
            }
        }
        // 내가 시민이면
        else
        {
            if (result == MafiaResult.MafiaWin)
            {
                winLoseUI.ShowLose(LOSEPOINTS);
            }
            else
            {
                winLoseUI.ShowWin(WINPOINTS);
            }
        }
        
        yield return new WaitForSeconds(3);

        LeaveGame();
    }
    #endregion

    private void LeaveGame()
    {
        // Release singleton instance
        Singleton<MafiaManager>.ReleaseInstance();

        if (PhotonNetwork.IsMasterClient)
        {
            // Reset every player's dead state
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                player.SetDead(false);
            }

            // Load Menu Scene
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(MenuSceneName);
        }
    }
}