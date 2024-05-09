using System;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

public enum Phase { Day, Night }
public class PhaseManager : MonoBehaviour
{
    [SerializeField] private Phase curPhase;
    [SerializeField] private GameTimer timer;
    [SerializeField] private LightController light;

    [SerializeField] private int roleUseTime;
    [SerializeField] private int voteTime;
    
    private void Start()
    {
        // Game Start
        curPhase = Phase.Day;
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
        timer.StartTimer(roleUseTime);
    }

    public void SetTimes( int roleUseTime, int voteTime )
    {
        this.roleUseTime = roleUseTime;
        this.voteTime = voteTime;
    }

}
