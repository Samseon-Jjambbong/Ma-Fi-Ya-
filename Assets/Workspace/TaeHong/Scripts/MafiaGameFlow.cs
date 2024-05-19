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

    [PunRPC]
    public void ShowVoteResults()
    {
        StartCoroutine(ShowVoteResultsRoutine());
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
        Manager.Mafia.ActivateHouseOutlines();

        yield return timer.StartTimer(time);

        Manager.Mafia.DeactivateHouseOutlines();

        // Store skill usage info
        Manager.Mafia.NotifyAction();

        yield return new WaitForSeconds(3); // Give time for network to receive actions

        //REMOVE LATER
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.developerConsoleVisible = true;
            Debug.Log($"Begin ActionPQ Debug with Count: {Manager.Mafia.MafiaActionPQ.Count}");
            while (Manager.Mafia.MafiaActionPQ.Count > 0)
            {
                MafiaAction action = Manager.Mafia.MafiaActionPQ.Dequeue();
                Debug.Log($"{action.sender} ==> {action.receiver} with {action.actionType}");
            }
            Debug.Log($"End ActionPQ Debug");
        }

        Debug.Log("Mafia Chat disabled");
        // TODO : Insert chat OFF function here
    }

    private IEnumerator ShowNightEventsRoutine()
    {
        Manager.Mafia.photonView.RPC("ParseActionsAndAssign", RpcTarget.MasterClient);
        yield return new WaitForSeconds(1);
        Manager.Mafia.Player.photonView.RPC("ShowActions", RpcTarget.All);
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
            Manager.Mafia.Houses[i].ShowVoteCount(true);
            if ( i == (PhotonNetwork.LocalPlayer.ActorNumber - 1) )
                continue;
            
            Manager.Mafia.Houses[i].ActivateOutline(true);
        }

        yield return timer.StartTimer(time);
        
        foreach ( var house in Manager.Mafia.Houses )
        {
            house.ActivateOutline(false);
            house.ShowVoteCount(false);
        }

        EnableChat(false);
    }

    private IEnumerator ShowVoteResultsRoutine()
    {
        // Show vote result on everyone's screen
        int voteResult = Manager.Mafia.GetVoteResult();
        if(voteResult == -1)
        {
            Debug.Log("No one got kicked");
        }
        else
        {
            Debug.Log($"Player{voteResult} got kicked");
        }
        yield return new WaitForSeconds(1);
    }

    #endregion
}
