using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Tae;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MafiaGameFlow : MonoBehaviourPun
{
    [SerializeField] private GameTimer timer;
    [SerializeField] private LightController lightController;
    [SerializeField] private GameObject roleUI;
    [SerializeField] private TextMeshProUGUI resultsText;

    private void Start()
    {
        Manager.Mafia.IsDay = true;
    }

    #region RPCs
    [PunRPC]
    public void DisplayRole(int time)
    {
        StartCoroutine(DisplayRoleRoutine(time));
    }

    [PunRPC]
    public void StartNightPhase(int time)
    {
        StartCoroutine(NightPhaseRoutine(time));
    }

    [PunRPC]
    public void ShowNightEvents()
    {
        StartCoroutine(ShowNightEventsRoutine());
    }

    [PunRPC]
    public void StartDayPhase(int time)
    {
        StartCoroutine(DayPhaseRoutine(time));
    }

    public void ChangeTime()
    {
        StartCoroutine(ChangeTimeOfDayRoutine());
    }

    public void EnableChat(bool enable)
    {
        if (enable)
        {
            Debug.Log("Enabled Chat");
        }
        else
        {
            Debug.Log("Disabled Chat");
        }
    }

    

    #endregion

    #region Coroutines
    // Display Role for X seconds
    IEnumerator DisplayRoleRoutine(int time)
    {
        roleUI.SetActive(true);
        yield return timer.StartTimer(time);
        roleUI.SetActive(false);
    }
    
    // Day/Night Light Changer
    private IEnumerator ChangeTimeOfDayRoutine()
    {
        yield return lightController.ChangePhase();
        Manager.Mafia.IsDay = !Manager.Mafia.IsDay;
    }
    
    // Night Phase
    private IEnumerator NightPhaseRoutine(int time)
    {
        // Day -> Night
        yield return ChangeTimeOfDayRoutine();

        // Allow chat for mafia
        Debug.Log("Mafia Chat enabled");
        // TODO : Insert chat ON function here

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
        // TODO : Insert chat OFF function here
    }

    private IEnumerator ShowNightEventsRoutine()
    {
        Manager.Mafia.Player.photonView.RPC("ShowNightResults", RpcTarget.All);
        yield return new WaitForSeconds(1);
    }

    // Allow Chat and votingfor X Seconds
    private IEnumerator DayPhaseRoutine(int time)
    {
        // Night -> Day
        yield return ChangeTimeOfDayRoutine();

        // Allow chat for everyone
        EnableChat(true);

        // Allow voting for X Seconds
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

        EnableChat(false);
    }

    #endregion
}
