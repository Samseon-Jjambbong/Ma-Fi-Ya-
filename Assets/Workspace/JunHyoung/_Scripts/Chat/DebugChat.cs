using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

public class DebugChat : MonoBehaviourPunCallbacks
{
    [SerializeField]  string USERNAME ;
    [SerializeField]  string ROOMNAME ;

   
    private void Awake()
    {
        if ( PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected ) // 접속중이 아닐 때만 로그인
            return;

        PhotonNetwork.LocalPlayer.NickName = USERNAME;
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 8 };
        options.SetGameMode(GameMode.Mafia, true);
        PhotonNetwork.CreateRoom(roomName: ROOMNAME, roomOptions: options);
    }
}
