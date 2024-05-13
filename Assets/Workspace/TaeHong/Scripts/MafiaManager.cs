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
    [SerializeField] private int roleUseTime;
    [SerializeField] private int voteTime;
    [SerializeField] private float skillTime;
    public float SkillTime => skillTime;

    private void Start()
    {
        Debug.Log("In");

        isDay = true;
        // timer.StartTimer(roleUseTime);
        Debug.Log(PhotonNetwork.CurrentRoom.Players.Count);
        playerCount = PhotonNetwork.CurrentRoom.Players.Count;
    }
    
    private void OnEnable()
    {
        timer.TimerFinished += OnTimerFinished;
    }

    private void OnDisable()
    {
        timer.TimerFinished += OnTimerFinished;
    }

    private void OnTimerFinished()
    {
        // light.ChangePhase(); 
        isDay = !isDay;
        timer.StartTimer(roleUseTime);
    }

    public void SetTimes( int roleUseTime, int voteTime )
    {
        this.roleUseTime = roleUseTime;
        this.voteTime = voteTime;
    }
}
