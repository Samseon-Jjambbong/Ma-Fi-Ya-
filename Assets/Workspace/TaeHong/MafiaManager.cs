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
    [SerializeField] private int playerCount;
    public int PlayerCount => playerCount;

    private bool isDay;
    public bool IsDay => isDay;
    [SerializeField] private GameTimer timer;
    [SerializeField] private Tae.LightController light;
    [SerializeField] private int roleUseTime;
    [SerializeField] private int voteTime;

    private void Start()
    {
        isDay = true;
        timer.StartTimer(roleUseTime);
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
        light.ChangePhase();
        isDay = !isDay;
        timer.StartTimer(roleUseTime);
    }

    public void SetTimes( int roleUseTime, int voteTime )
    {
        this.roleUseTime = roleUseTime;
        this.voteTime = voteTime;
    }
}
