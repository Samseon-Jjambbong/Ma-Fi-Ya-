using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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
        if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected) // 접속중이 아닐 때만 로그인
            return;
        USERNAME = $"DebugPlayer {Random.Range(1000, 10000)}";
        PhotonNetwork.LocalPlayer.NickName = USERNAME;
        PhotonNetwork.ConnectUsingSettings();

        if (debugButton1 != null)
            debugButton1.onClick.AddListener(PlayerKill);
        if (debugButton2 != null)
            debugButton2.onClick.AddListener(PlayerDeath);
    }


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
    }

    void PlayerKill()
    {
        Debug.Log($"cut Kill Count {PhotonNetwork.LocalPlayer.GetPlayerKillCount()}");
        PhotonNetwork.LocalPlayer.AddPlayerKillCount(); 
    }

    private void PlayerDeath()
    {
        Debug.Log($"cur Death Count {PhotonNetwork.LocalPlayer.GetPlayerDeathCount()}");
        PhotonNetwork.LocalPlayer.AddPlayerDeathCount();
    }
}