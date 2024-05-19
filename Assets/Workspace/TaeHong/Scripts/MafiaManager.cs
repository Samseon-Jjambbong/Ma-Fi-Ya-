using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Manager for Mafia game mode.
/// </summary>

public class MafiaManager : Singleton<MafiaManager>, IPunObservable
{
    private int playerCount;
    public int PlayerCount => playerCount;

    private bool isDay;
    public bool IsDay { get; set; }
    [SerializeField] private int displayRoleTime;
    [SerializeField] private int roleUseTime;
    [SerializeField] private int voteTime;
    [SerializeField] private float skillTime;

    [SerializeField] GameObject nightMafia;
    public GameObject NightMafia => nightMafia;
    [SerializeField] Vector3 nightMafiaPos;
    public Vector3 NightMafiaPos => nightMafiaPos;

    [SerializeField] List<House> houses;
    public List<House> Houses { get { return houses; } set { houses = value; } }
    public float SkillTime => skillTime;

    private MafiaPlayer player;
    public MafiaPlayer Player { get; set; }

    // MASTER CLIENT ONLY
    public MafiaGame Game = new MafiaGame();
    public event Action VoteCountChanged;
    private int[] votes;
    public int[] Votes => votes;

    private MafiaAction? playerAction;
    public MafiaAction? PlayerAction { get { return playerAction; } set { playerAction = value; } }

    public MafiaActionPQ MafiaActionPQ = new MafiaActionPQ();
    private bool[] blockedPlayers;
    private bool[] deadPlayers;
    public bool[] healedPlayers;
    public PhotonView photonView => GetComponent<PhotonView>();

    private void Start()
    {
        isDay = true;
        playerCount = PhotonNetwork.CurrentRoom.Players.Count;
        votes = new int[playerCount];
        blockedPlayers = new bool[playerCount];
        deadPlayers = new bool[playerCount];
        healedPlayers = new bool[playerCount];
    }

    [PunRPC] // Called only on MasterClient
    public void PlayerDied(int id)
    {
        bool result = Game.RemovePlayer(PhotonNetwork.PlayerList[id].GetPlayerRole());
        if (result)
        {
            // civilian win
        }
        else
        {
            // mafia win
        }
    }

    [PunRPC]
    public void VoteForPlayer(int playerID)
    {
        votes[playerID - 1]++;
        VoteCountChanged?.Invoke();
    }

    public int GetVoteResult() // Return playerID or -1 if none
    {
        int maxVotes = -1;
        int maxVoted = -1;
        for (int i = 0; i < votes.Length; i++)
        {
            if (votes[i] > maxVotes)
            {
                maxVotes = votes[i];
                maxVoted = i + 1;
            }
        }

        // Reset values
        for (int i = 0; i < votes.Length; i++)
        {
            votes[i] = 0;
        }

        return maxVoted;
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

    public void ShowMyPlayerMove(House house)
    {
        GameObject obj = Instantiate(Manager.Mafia.NightMafia, Manager.Mafia.NightMafiaPos, Manager.Mafia.NightMafia.transform.rotation);

        foreach (MafiaPlayer player in FindObjectsOfType<MafiaPlayer>())
        {
            if (player.IsMine)
            {
                obj.GetComponentInChildren<Renderer>().material.color = player.GetComponentInChildren<Renderer>().material.color;
            }
        }
        NightMafiaMove mafia = obj.GetComponent<NightMafiaMove>();

        mafia.Target = house.gameObject;
        mafia.MoveToTarget();
    }

    public void ShowSomebodyMove(House house)
    {
        house.MafiaComesHome();
    }

    public void ActivateHouseOutlines()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (i == (PhotonNetwork.LocalPlayer.ActorNumber - 1))
                continue;

            Manager.Mafia.Houses[i].ActivateOutline(true);
        }
    }

    public void DeactivateHouseOutlines()
    {
        foreach (var house in Manager.Mafia.Houses)
        {
            house.ActivateOutline(false);
        }
    }

    public void NotifyAction()
    {
        if (PlayerAction == null)
        {
            Debug.Log("Notify action null");
            return;
        }

        MafiaAction action = (MafiaAction) PlayerAction;
        photonView.RPC("EnqueueAction", RpcTarget.MasterClient, action.Serialize());
    }

    [PunRPC] // Called only on MasterClient
    public void EnqueueAction(int[] serialized)
    {
        Debug.Log("enqueue action called");
        MafiaAction action = new MafiaAction(serialized);
        MafiaActionPQ.Enqueue(action);
    }

    [PunRPC] // Called only on MasterClient
    public void ParseActionsAndAssign()
    {
        MafiaAction action;
        while (MafiaActionPQ.Count > 0)
        {
            action = MafiaActionPQ.Dequeue();
            int senderIdx = action.sender - 1;
            int receiverIdx = action.receiver - 1;

            // If blocked, don't add action
            if (blockedPlayers[senderIdx])
            {
                continue;
            }

            // Send action info to players
            switch (action.actionType)
            {
                case MafiaActionType.Block:
                    blockedPlayers[receiverIdx] = true;
                    Player.photonView.RPC("AddAction", PhotonNetwork.PlayerList[senderIdx], action.Serialize());
                    Player.photonView.RPC("AddAction", PhotonNetwork.PlayerList[receiverIdx], action.Serialize());
                    break;
                case MafiaActionType.Kill:
                    deadPlayers[receiverIdx] = true;
                    Player.photonView.RPC("AddAction", PhotonNetwork.PlayerList[senderIdx], action.Serialize());
                    Player.photonView.RPC("AddAction", PhotonNetwork.PlayerList[receiverIdx], action.Serialize());
                    break;
                case MafiaActionType.Heal:
                    if (deadPlayers[receiverIdx] == true)
                    {
                        deadPlayers[receiverIdx] = false;
                        healedPlayers[receiverIdx] = true;
                    }
                    Player.photonView.RPC("AddAction", PhotonNetwork.PlayerList[senderIdx], action.Serialize());
                    Player.photonView.RPC("AddAction", PhotonNetwork.PlayerList[receiverIdx], action.Serialize());
                    break;
            }
        }

        // Reset states
        blockedPlayers = new bool[playerCount];
        deadPlayers = new bool[playerCount];
        healedPlayers = new bool[playerCount];
    }
}
