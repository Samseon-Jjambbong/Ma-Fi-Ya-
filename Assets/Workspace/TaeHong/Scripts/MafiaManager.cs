using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;

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

    public MafiaGame Game = new MafiaGame();
    public event Action VoteCountChanged;
    private int[] votes;
    public int[] Votes => votes;

    private void Start()
    {
        isDay = true;
        playerCount = PhotonNetwork.CurrentRoom.Players.Count;
        votes = new int[playerCount];
    }

    [PunRPC] // Called only on MasterClient
    public void PlayerDied(int id)
    {
        bool result = Game.RemovePlayer((MafiaRole)PhotonNetwork.CurrentRoom.GetMafiaRoleList()[id - 1]);
        if(result)
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
        for(int i = 0; i < votes.Length; i++)
        {
            if (votes[i] > maxVotes)
            {
                maxVotes = votes[i];
                maxVoted = i + 1;
            }
        }

        // Reset values
        for(int i = 0; i < votes.Length; i++)
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
            isDay = (bool)stream.ReceiveNext();
        }
    }
}
