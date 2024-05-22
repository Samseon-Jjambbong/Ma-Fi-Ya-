using Mafia;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tae;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MafiaGameFlow : MonoBehaviourPun
{
    [SerializeField] private GameTimer timer;
    [SerializeField] private LightController lightController;
    [SerializeField] private RoleUI roleUI;
    [SerializeField] private Button skipVoteButton;
    [SerializeField] private WinLoseUI winLoseUI;
    [SerializeField] string MenuSceneName;

    private void Start()
    {
        Manager.Mafia.IsDay = true;
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
    public void ShowVoteResults()
    {
        StartCoroutine(ShowVoteResultsRoutine());
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
        if (enable)
        {
            // TODO: Enable chat here
        }
        else
        {
            // TODO: Disable chat here
        }
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
        Manager.Mafia.displayRoleFinished = true;
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
        Manager.Mafia.IsDay = !Manager.Mafia.IsDay;
        if (!Manager.Mafia.IsDay)
        {
            if (!PhotonNetwork.LocalPlayer.GetPlayerRole().Equals(MafiaRole.Mafia) && Manager.Mafia.Player.IsAlive)
            {
                InGameChatManager.Instance.IsChatable = false;
            }
        }
        else
        {
            if (!InGameChatManager.Instance.IsChatable)
            {
                InGameChatManager.Instance.IsChatable = true;
            }
        }
    }

    // Night Phase
    private IEnumerator NightPhaseRoutine(int time)
    {
        // Day -> Night
        yield return ChangeTimeOfDayRoutine();

        // Allow chat for mafia
        // TODO : Insert chat ON function here

        // Allow skill usage for X Seconds
        Manager.Mafia.ActivateHouseOutlines();

        yield return timer.StartTimer(time);

        Manager.Mafia.DeactivateHouseOutlines();

        // Store skill usage info
        Manager.Mafia.NotifyAction();

        yield return new WaitForSeconds(3); // Give time for network to receive actions

        // TODO : Insert chat OFF function here

        Manager.Mafia.nightPhaseFinished = true;
    }

    private IEnumerator ShowNightEventsRoutine()
    {
        Manager.Mafia.sharedData.photonView.RPC("ResetClientFinishedCount", RpcTarget.All);
        Manager.Mafia.photonView.RPC("ParseActionsAndAssign", RpcTarget.MasterClient);
        yield return new WaitForSeconds(1);
        Manager.Mafia.photonView.RPC("ShowActions", RpcTarget.All);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => Manager.Mafia.sharedData.clientFinishedCount == Manager.Mafia.ActivePlayerCount());
        Manager.Mafia.nightEventsFinished = true;
        Manager.Mafia.sharedData.photonView.RPC("ClearActionInfo", RpcTarget.All);
    }

    private IEnumerator ShowNightResultsRoutine()
    {
        // Night -> Day
        yield return ChangeTimeOfDayRoutine();

        // Show Players that died last night
        List<int> killed = Manager.Mafia.sharedData.GetKilledPlayers();
        if (killed.Count == 0)
        {
            Debug.Log("No one died last night");
        }
        else
        {
            // Show player die animation
            yield return Manager.Mafia.ShowKilledPlayers(killed);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        Manager.Mafia.nightResultsFinished = true;
    }

    // Allow Chat and voting for X Seconds
    private IEnumerator DayPhaseRoutine(int time)
    {
        // Allow chat for everyone
        EnableChat(true);

        // Allow voting for X Seconds
        skipVoteButton.gameObject.SetActive(true);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Manager.Mafia.Houses[i].ShowVoteCount(true);
            if (i == (PhotonNetwork.LocalPlayer.ActorNumber - 1))
                continue;

            Manager.Mafia.Houses[i].ActivateOutline(true);
        }

        yield return timer.StartTimer(time);

        skipVoteButton.gameObject.SetActive(false);
        foreach (var house in Manager.Mafia.Houses)
        {
            house.ActivateOutline(false);
            house.ShowVoteCount(false);
        }

        EnableChat(false);
        Manager.Mafia.photonView.RPC("CountVotes", RpcTarget.MasterClient);
        Manager.Mafia.dayPhaseFinished = true;
    }

    private IEnumerator ShowVoteResultsRoutine()
    {
        // Show vote result on everyone's screen
        int voteResult = Manager.Mafia.sharedData.playerToKick;
        Debug.Log($"Vote Result : {voteResult}");
        if (voteResult == -1)
        {
            Debug.Log("No one got kicked");
            Manager.Mafia.voteResultsFinished = true;
            yield break;
        }
        else
        {
            Debug.Log($"Player{voteResult} got kicked");
            // Insert Player kicked coroutine here
            yield return Manager.Mafia.animFactory.PlayerKickedActionRoutine(voteResult);
            yield return new WaitUntil(() => Manager.Mafia.voteResultsFinished);
            yield return RemovedPlayerRoleRoutine(voteResult);
            Manager.Mafia.sharedData.playerToKick = -1; // Reset value
        }
    }

    private IEnumerator GameOverRoutine(int gameResult)
    {
        MafiaResult result = (MafiaResult) gameResult;
        // 내가 마피아면
        if (PhotonNetwork.LocalPlayer.GetPlayerRole() == MafiaRole.Mafia)
        {
            if (result == MafiaResult.MafiaWin)
            {
                winLoseUI.ShowWin(100);
            }
            else
            {
                winLoseUI.ShowLose(100);
            }
        }
        // 내가 시민이면
        else
        {
            if (result == MafiaResult.MafiaWin)
            {
                winLoseUI.ShowLose(100);
            }
            else
            {
                winLoseUI.ShowWin(100);
            }
        }
        
        yield return new WaitForSeconds(3);

        // Go back to lobby scene
        Manager.Scene.LoadScene(MenuSceneName);
    }

    #endregion
}