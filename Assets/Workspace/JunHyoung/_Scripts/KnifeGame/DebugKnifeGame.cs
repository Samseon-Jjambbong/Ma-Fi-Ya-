using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DebugKnifeGame : MonoBehaviourPunCallbacks
{
    [SerializeField] string USERNAME;
    [SerializeField] string ROOMNAME;

    [SerializeField] KnifeGameChatManager chatManager;
    [SerializeField] KnifeGameManager knifeGameManager;


    [SerializeField] Button debugButton1;
    [SerializeField] Button debugButton2;

    private void Awake()
    {
#if UNITY_EDITOR
        DebugConnect();
#endif
        if (debugButton1 != null)
            debugButton1.onClick.AddListener(PlayerKill);
        if (debugButton2 != null)
            debugButton2.onClick.AddListener(PlayerDeath);
    }

    void DebugConnect()
    {
        if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected) // 접속중이 아닐 때만 로그인
            return;
        USERNAME = $"DebugPlayer {Random.Range(1000, 10000)}";
        PhotonNetwork.LocalPlayer.NickName = USERNAME;
        PhotonNetwork.ConnectUsingSettings();
    }
#if UNITY_EDITOR
    public override void OnConnectedToMaster()
    {

        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        options.SetGameMode(GameMode.Knife, true);
        PhotonNetwork.JoinOrCreateRoom(roomName: ROOMNAME, roomOptions: options, TypedLobby.Default);

    }

    public override void OnJoinedRoom()
    {

        chatManager.gameObject.SetActive(true);
        knifeGameManager.gameObject.SetActive(true);
#endif
    }

    void PlayerKill()
    {
        PhotonNetwork.LocalPlayer.AddPlayerKillCount(); 
    }

    private void PlayerDeath()
    {
        PhotonNetwork.LocalPlayer.AddPlayerDeathCount();
    }
}