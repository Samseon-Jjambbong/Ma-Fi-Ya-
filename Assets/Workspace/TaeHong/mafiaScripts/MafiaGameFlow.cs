using System;
using System.Collections;
using System.Collections.Generic;
using Tae;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MafiaGameFlow : MonoBehaviour
{
    [SerializeField] private GameTimer timer;
    [SerializeField] private LightController lightController;
    [SerializeField] private GameObject roleUI;
    private float displayRoleDuration = 3f;
    private float chatDuration = 3f;

    private void Start()
    {
        
    }

    // Display Role for X seconds
    IEnumerator DisplayRoleRoutine()
    {
        roleUI.SetActive(true);
        yield return new WaitForSeconds(displayRoleDuration);
        roleUI.SetActive(true);
    }
    
    // Allow Chat for X Seconds
    IEnumerator AllowChatRoutine()
    {
        // enable Chat
        Debug.Log("Chat enabled");
        
        yield return new WaitForSeconds(chatDuration);
        
        // disable Chat
        Debug.Log("Chat disabled");
    }
    
    // Day -> Night
    private void ChangeIntoNight()
    {
        lightController.ChangePhase();
    }
    
    // Night Stuff
    // Allow chat for mafia
    // Allow skill usage for X Seconds
    
    // Night -> Day
    // Allow Chat for X Seconds
    // Show role usage results
    // Show player death (if there was any)
    
    // Repeat
}
