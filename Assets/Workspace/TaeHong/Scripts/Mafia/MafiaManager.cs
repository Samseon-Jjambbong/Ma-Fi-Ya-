using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;
using TMPro;
using Mafia;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Manager for Mafia game mode.
/// </summary>

public class MafiaManager : Singleton<MafiaManager>, IPunObservable
{
    [Header("Components")]
    public SharedData sharedData;
    public AnimationFactory animFactory;
    private MafiaGameFlow gameFlow;
    public PhotonView photonView => GetComponent<PhotonView>();
    public JobToolTip toolTip;
    public CurrentJobPanel jobPanel;
    private int playerCount;
    public int PlayerCount => playerCount;

    private bool isDay;
    public bool IsDay { get => isDay; set => isDay = value; }
    [SerializeField] private float skillTime;

    [SerializeField] List<House> houses;
    public List<House> Houses { get { return houses; } set { houses = value; } }
    public float SkillTime => skillTime;

    private MafiaPlayer player;
    public MafiaPlayer Player { get { return player; } set { player = value; } }

    private House house;
    public House House { get; set; }

    [Header("Game Logic")]
    public MafiaGame Game = new MafiaGame();
    private MafiaResult gameResult = MafiaResult.None;
    public MafiaResult GameResult => gameResult;
    public event Action VoteCountChanged;
    public event Action SkipVoteCountChanged;
    private int[] votes;
    public int[] Votes => votes;
    private int skipVotes;
    public int SkipVotes => skipVotes;

    private MafiaAction? playerAction;
    public MafiaAction? PlayerAction { get { return playerAction; } set { playerAction = value; } }

    public MafiaActionPQ MafiaActionPQ = new MafiaActionPQ();

    // Game Loop Flags
    public bool displayRoleFinished;
    public bool nightPhaseFinished;
    public bool nightEventsFinished;
    public bool nightResultsFinished;
    public int voteCount;
    public bool dayPhaseFinished;
    public bool voteResultsFinished;

    private void Start()
    {
        gameFlow = GetComponent<MafiaGameFlow>();
        isDay = true;
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        votes = new int[playerCount];
    }

    public void ResetFlags()
    {
        //displayRoleFinished = false;
        //nightPhaseFinished = false;
        //nightEventsFinished = false;
        //nightResultsFinished = false;
        //dayPhaseFinished = false;
        //voteResultsFinished = false;
    }

    public void ShowRoleList()
    {
        jobPanel.InitJobPanel();
    }

    public int ActivePlayerCount()
    {
        return sharedData.ActivePlayerCount();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isDay);
        }
        else
        {
            isDay = (bool) stream.ReceiveNext();
        }
    }

    /******************************************************
    *                    Morning
    ******************************************************/
    #region Last Night Results
    public IEnumerator ShowKilledPlayers(List<int> killed)
    {
        Debug.Log($"{killed.Count} killed last night");
        foreach (int playerID in killed)
        {
            Debug.Log($"Player{playerID} got killed");
            // Show dying animation
            yield return animFactory.SpawnPlayerDie(Houses[playerID - 1]);

            yield return new WaitForSeconds(1);

            // Show everyone dead player's role
            yield return gameFlow.RemovedPlayerRoleRoutine(playerID);

            // Set player state as dead
            sharedData.SetDead(playerID - 1);

            // Disable chat for kicked player
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
            {
                MafiaGameChatManager.Instance.SubscribleGhostChannel();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PlayerDied(playerID);
            }

            yield return new WaitForSeconds(1);
        }
    }
    #endregion

    #region Voting
    [PunRPC]
    public void VoteForPlayer(int playerID)
    {
        voteCount++;
        if (playerID == -1)
        {
            skipVotes++;
            SkipVoteCountChanged?.Invoke();
            return;
        }
        votes[playerID - 1]++;
        VoteCountChanged?.Invoke();
    }

    [PunRPC] // Called on players who finished voting
    public void BlockVotes()
    {
        DeactivateHouseUIs();
        gameFlow.DisableSkipButton();
    }

    [PunRPC] // Called only on MasterClient
    public void CountVotes() // Return playerID or -1 if none
    {
        // DEBUG:
        Debug.Log("Counting Votes:");
        for (int i = 0; i < votes.Length; i++)
        {
            Debug.Log($"Player {i + 1} got {votes[i]} votes.");
        }

        // Look for candidate with highest votes
        int highestIdx = 0;
        int count = 1;
        int voted = votes[0];
        for(int i = 1; i < votes.Length; i++)
        {
            if (votes[i] > votes[highestIdx])
            {
                highestIdx = i;
                count = 1;
            }
            else if (votes[i] == votes[highestIdx])
            {
                count++;
            }
            voted += votes[i];
        }
        // Return result
        // No one gets kicked if:
        //      - There is a tie for highest votes
        //      - Skipped votes > highest vote
        int result;
        if (count > 1 || skipVotes > votes[highestIdx])
        {
            result = -1;
        }
        else
        {
            result = highestIdx + 1;
        }
        Debug.Log($"Result: {result}");

        photonView.RPC("ResetVotes", RpcTarget.All);
        sharedData.photonView.RPC("SetPlayerToKick", RpcTarget.All, result);
    }

    [PunRPC]
    public void ResetVotes()
    {
        // Reset values before returning result
        for (int i = 0; i < votes.Length; i++)
        {
            votes[i] = 0;
        }
        skipVotes = 0;
    }
    #endregion

    #region Vote Result
    public void ApplyVoteResult(int voteResult)
    {
        // Disable chat for kicked player
        if(PhotonNetwork.LocalPlayer.ActorNumber == voteResult)
        {
            MafiaGameChatManager.Instance.SubscribleGhostChannel();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerDied(voteResult);
            sharedData.playerToKick = -1; // Reset value
        }
    }
    #endregion

    /******************************************************
    *                    Night
    ******************************************************/
    #region Player Actions
    public void NotifyAction()
    {
        if (PlayerAction == null)
        {
            return;
        }

        MafiaAction action = (MafiaAction) PlayerAction;
        photonView.RPC("EnqueueAction", RpcTarget.MasterClient, action.Serialize());
        PlayerAction = null;
    }

    [PunRPC] // Called only on MasterClient
    public void EnqueueAction(int[] serialized)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        MafiaAction action = new MafiaAction(serialized);
        MafiaActionPQ.Enqueue(action);
    }

    [PunRPC] // Called only on MasterClient
    public void ParseActionsAndAssign()
    {
        sharedData.photonView.RPC("ResetPlayerStates", RpcTarget.All);

        MafiaAction action;
        
        Debug.Log($"Begin ActionPQ Debug with Count: {MafiaActionPQ.Count}");
        while (MafiaActionPQ.Count > 0)
        {
            action = MafiaActionPQ.Dequeue();
            Debug.Log($"{action.sender} ==> {action.receiver} with {action.actionType}");
            int senderIdx = action.sender - 1;
            int receiverIdx = action.receiver - 1;

            // If blocked, don't add action
            if (sharedData.blockedPlayers[senderIdx])
            {
                continue;
            }

            // Send action info to players (if not insane)
            if (PhotonNetwork.CurrentRoom.Players[action.sender].GetPlayerRole() != MafiaRole.Insane)
            {
                switch (action.actionType)
                {
                    case MafiaActionType.Block:
                        sharedData.photonView.RPC("SetBlocked", RpcTarget.All, receiverIdx, true);
                        break;
                    case MafiaActionType.Kill:
                        sharedData.photonView.RPC("SetKilled", RpcTarget.All, receiverIdx, true);
                        break;
                    case MafiaActionType.Heal:
                        if (sharedData.killedPlayers[receiverIdx] == true)
                        {
                            sharedData.photonView.RPC("SetKilled", RpcTarget.All, receiverIdx, false);
                            sharedData.photonView.RPC("SetHealed", RpcTarget.All, receiverIdx, true);
                        }
                        break;
                }
            }

            // Add action to shared data
            sharedData.photonView.RPC("AddAction", RpcTarget.All, action.Serialize());
        }
        Debug.Log($"End ActionPQ Debug");
    }

    [PunRPC]
    public void ShowActions()
    {
        StartCoroutine(Player.ShowActionsRoutine());
    }
    #endregion

    /******************************************************
    *                    Game Over
    ******************************************************/
    #region Game Over
    // Called only on MasterClient
    public void PlayerDied(int id)
    {
        PhotonNetwork.CurrentRoom.Players[id].SetDead(true);
        Debug.Log($"Game result before: {gameResult}, Mafias : {Game.NumMafias}, Civs : {Game.NumCivs}");
        gameResult = Game.RemovePlayer(PhotonNetwork.CurrentRoom.Players[id].GetPlayerRole());
        Debug.Log($"Game result after: {gameResult}, Mafias : {Game.NumMafias}, Civs : {Game.NumCivs}");
    }
    #endregion

    /******************************************************
    *                    Utils
    ******************************************************/
    #region Utils
    public void ActivateHouseOutlines()
    {
        for (int i = 0; i < PlayerCount; i++)
        {
            // Skip if
            // house is client's
            if (i == (PhotonNetwork.LocalPlayer.ActorNumber - 1))
                continue;
            // house owner is dead
            if (PhotonNetwork.CurrentRoom.Players[i + 1].GetDead())
                continue;

            Houses[i].ActivateOutline();
        }
    }

    public void DeactivateHouseOutlines()
    {
        foreach (var house in Manager.Mafia.Houses)
        {
            house.DeactivateOutline();
        }
    }

    public void DeactivateHouseUIs()
    {
        foreach (var house in Manager.Mafia.Houses)
        {
            house.HideUI();
        }
    }
    #endregion
}
