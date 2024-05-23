using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillLogManager : MonoBehaviour, IOnEventCallback
{
    [Header("KillLog")]
    public const byte KillLogEventCode = 44;
    [SerializeField] KillLogEntry killLogEntry;
    [SerializeField] Transform contents; //killLogEntry가 생성될 부모

    void Awake()
    {
        PhotonPeer.RegisterType(typeof(KillLogData), (byte) 'K', KillLogData.Serialize, KillLogData.Deserialize);
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /******************************************************
    *                 IOnEventCallback
    ******************************************************/
    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == KillLogEventCode)
        {
            KillLogData log = (KillLogData)photonEvent.CustomData;
            KillLogEntry newLog = Instantiate(killLogEntry,contents);
            newLog.SetEntry(log);
        }


        // RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        // KillLogData log = new KillLogData("name", "target", 1); // if you Kill
        // KillLogData log = new KillLogData("name"); //if You Death 
        // PhotonNetwork.RaiseEvent(KillLogEventCode, log, raiseEventOptions, SendOptions.SendReliable);
    }
}
