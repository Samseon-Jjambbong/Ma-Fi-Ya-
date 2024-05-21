using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKnifeGame : MonoBehaviourPunCallbacks
{
    [SerializeField] string USERNAME;
    [SerializeField] string ROOMNAME;

    [SerializeField] MafiaGameChatManager chatManager;
    [SerializeField] KnifeGameManager knifeGameManager;

    private void Awake()
    {
        if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected) // 접속중이 아닐 때만 로그인
            return;

        PhotonNetwork.LocalPlayer.NickName = USERNAME;
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 1 };
        options.SetGameMode(GameMode.Knife, true);
        PhotonNetwork.JoinOrCreateRoom(roomName: ROOMNAME, roomOptions: options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        chatManager.gameObject.SetActive(true);
        knifeGameManager.gameObject.SetActive(true);
    }
}