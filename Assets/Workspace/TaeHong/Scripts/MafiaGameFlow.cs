using System;
using System.Collections;
using System.Collections.Generic;
using Tae;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MafiaGameFlow : Singleton<MafiaGameFlow>
{
    [SerializeField] private GameTimer timer;
    [SerializeField] private LightController lightController;
    [SerializeField] private GameObject roleUI;
    private int displayRoleDuration = 3;
    private int chatDuration = 3;
    private bool isDay;
    public List<House> Houses { get; set; }

    private void Start()
    {
        isDay = true;
        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        yield return new WaitForSeconds(1);
        yield return DisplayRoleRoutine();
        yield return AllowChatRoutine();
        yield return lightController.ChangePhase(isDay);
        isDay = false;
        //ChangeIntoNight();
        yield return NightRoutine();
        
        yield return lightController.ChangePhase(isDay);
        isDay = true;
    }

    // Display Role for X seconds
    IEnumerator DisplayRoleRoutine()
    {
        roleUI.SetActive(true);
        yield return timer.StartTimer(displayRoleDuration);
        roleUI.SetActive(false);
    }
    
    // Allow Chat for X Seconds
    IEnumerator AllowChatRoutine()
    {
        // enable Chat
        Debug.Log("Chat enabled");
        
        yield return timer.StartTimer(chatDuration);
        
        // disable Chat
        Debug.Log("Chat disabled");
    }
    
    // Day -> Night
    private void ChangeIntoNight()
    {
        lightController.ChangePhase(isDay);
        isDay = false;
    }
    
    // Night Stuff
    private IEnumerator NightRoutine()
    {
        // Allow chat for mafia
        Debug.Log("Mafia Chat enabled");

        // Allow skill usage for X Seconds
        foreach ( var house in Houses )
        {
            house.ActivateOutline(true);
        }

        yield return timer.StartTimer(chatDuration);
        
        foreach ( var house in Houses )
        {
            house.ActivateOutline(false);
        }
        
        Debug.Log("Mafia Chat disabled");
    }
    
    // Night -> Day
    // Allow Chat for X Seconds
    // Show role usage results
    // Show player death (if there was any)
    
    // Repeat
}
