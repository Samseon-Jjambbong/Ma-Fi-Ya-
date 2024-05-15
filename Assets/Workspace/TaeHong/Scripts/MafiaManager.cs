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

    [SerializeField] List<House> houses;
    public List<House> Houses { get { return houses; } set { houses = value; } }
    public float SkillTime => skillTime;

    private void Start()
    {
        isDay = true;
        // timer.StartTimer(roleUseTime);
        playerCount = PhotonNetwork.CurrentRoom.Players.Count;
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

    // TIME MANAGER:
    /*private PhotonView photonView;
    public void StartGame()
    {
        if ( !PhotonNetwork.IsMasterClient )
            return;
        
        photonView = GetComponent<PhotonView>();
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        // Delay
        yield return new WaitForSeconds(1);
        
        // Display role
        photonView.RPC("DisplayRole", RpcTarget.All, displayRoleTime);
        yield return new WaitForSeconds(displayRoleTime);

        // Delay
        yield return new WaitForSeconds(1);
        
        // Day Phase
        photonView.RPC("EnableChat", RpcTarget.All); // Enable Chat
        yield return new WaitForSeconds(voteTime);
        
        // Delay
        yield return new WaitForSeconds(1);
        
        // Change to night
        photonView.RPC("ChangeTime", RpcTarget.All);
        yield return new WaitForSeconds(1);
        
        // Allow role usage
        photonView.RPC("ChangeTime", RpcTarget.All, skillTime);
        photonView.RPC("EnableChat", RpcTarget.All);
    }*/
}
