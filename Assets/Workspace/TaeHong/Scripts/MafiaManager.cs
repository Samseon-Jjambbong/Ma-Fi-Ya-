using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Manager for Mafia game mode.
/// </summary>

public class MafiaManager : Singleton<MafiaManager>
{
    private int playerCount;
    public int PlayerCount => playerCount;

    private bool isDay;
    public bool IsDay => isDay;
    [SerializeField] private GameTimer timer;
    [SerializeField] private Tae.LightController light;
    [SerializeField] private int displayRoleTime;
    [SerializeField] private int roleUseTime;
    [SerializeField] private int voteTime;
    [SerializeField] private float skillTime;

    [SerializeField] List<House> houses;
    public List<House> Houses { get { return houses; } set { houses = value; } }
    public float SkillTime => skillTime;

    private void Start()
    {
        isDay = false;
        // timer.StartTimer(roleUseTime);
        playerCount = PhotonNetwork.CurrentRoom.Players.Count;
    }
    
    // private void OnEnable()
    // {
    //     timer.TimerFinished += OnTimerFinished;
    // }
    //
    // private void OnDisable()
    // {
    //     timer.TimerFinished += OnTimerFinished;
    // }
    //
    // private void OnTimerFinished()
    // {
    //     // light.ChangePhase(); 
    //     isDay = !isDay;
    //     timer.StartTimer(roleUseTime);
    // }
    //
    // public void SetTimes( int roleUseTime, int voteTime )
    // {
    //     this.roleUseTime = roleUseTime;
    //     this.voteTime = voteTime;
    // }
    
    // TIME MANAGER:
    private PhotonView photonView;
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
        timer.StartTimer(voteTime);
        
        // Delay
        yield return new WaitForSeconds(1);
        
        // Change to night
        photonView.RPC("ChangeTime", RpcTarget.All);
        yield return new WaitForSeconds(1);
        
        // Allow role usage
        photonView.RPC("ChangeTime", RpcTarget.All, skillTime);
        photonView.RPC("EnableChat", RpcTarget.All);
    }
}
