using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;
using UnityEngine.Serialization;

public class MafiaGameFlow : MonoBehaviourPun
{
    [SerializeField] private GameTimer timer;
    [SerializeField] private LightController lightController;
    [SerializeField] private GameObject roleUI;
    private bool isDay;

    // private void Start()
    // {
    //     isDay = true;
    //     StartCoroutine(GameFlow());
    // }

    public void TestGameFlow()
    {
        isDay = true;
        StartCoroutine(GameFlow());
    }

    [PunRPC]
    public void DisplayRole(int time)
    {
        StartCoroutine(DisplayRoleRoutine(time));
    }
    
    [PunRPC]
    public void AllowActions(int time)
    {
        StartCoroutine(NightRoutine(time));
    }

    [PunRPC]
    public void ChangeTime()
    {
        isDay = false;;
        StartCoroutine(ChangeTimeOfDayRoutine());
    }

    [PunRPC]
    public void EnableChat()
    {
        // Implement later
    }
    
    [PunRPC]
    public void DisableChat()
    {
        // Implement later
    }

    public void StartGameFlow()
    {
        isDay = true;
        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        // Delay
        yield return new WaitForSeconds(1);
        
        // Display Role for X seconds
        yield return DisplayRoleRoutine(3);

        // Loop
        while (true)
        {
            // Allow Chat for X Seconds
            yield return AllowChatRoutine();

            // Day -> Night
            yield return ChangeTimeOfDayRoutine();
            
            // Night Stuff
            yield return NightRoutine(5);

            // Night -> Day
            yield return ChangeTimeOfDayRoutine();

            //Day Stuff
            yield return DayRoutine();
        }
        
        //Show Results
    }

    // Display Role for X seconds
    IEnumerator DisplayRoleRoutine(int time)
    {
        roleUI.SetActive(true);
        yield return timer.StartTimer(time);
        roleUI.SetActive(false);
    }
    
    // Allow Chat for X Seconds
    IEnumerator AllowChatRoutine()
    {
        // enable Chat
        Debug.Log("Chat enabled");
        
        yield return timer.StartTimer(3);
        
        // disable Chat
        Debug.Log("Chat disabled");
    }
    
    // Day -> Night
    private IEnumerator ChangeTimeOfDayRoutine()
    {
        yield return lightController.ChangePhase(isDay);
        isDay = false;
    }
    
    // Night Stuff
    private IEnumerator NightRoutine(int time)
    {
        // Allow chat for mafia
        Debug.Log("Mafia Chat enabled");

        // Allow skill usage for X Seconds
        for ( int i = 0; i < PhotonNetwork.PlayerList.Length; i++ )
        {
            if ( i == (PhotonNetwork.LocalPlayer.ActorNumber - 1) )
                continue;
            
            Manager.Mafia.Houses[i].ActivateOutline(true);
        }

        yield return timer.StartTimer(time);

        foreach ( var house in Manager.Mafia.Houses )
        {
            house.ActivateOutline(false);
        }
        
        Debug.Log("Mafia Chat disabled");
    }
    
    // Night -> Day
    // Allow Chat for X Seconds
    // Show role usage results
    // Show player death (if there was any)
    private IEnumerator DayRoutine()
    {
        // Allow chat for mafia
        Debug.Log("Chat enabled");

        // Allow skill usage for X Seconds
        for ( int i = 0; i < PhotonNetwork.PlayerList.Length; i++ )
        {
            if ( i == (PhotonNetwork.LocalPlayer.ActorNumber - 1) )
                continue;
            
            Manager.Mafia.Houses[i].ActivateOutline(true);
        }

        yield return timer.StartTimer(20);
        
        foreach ( var house in Manager.Mafia.Houses )
        {
            house.ActivateOutline(false);
        }
        
        Debug.Log("Chat disabled");
    }
}
