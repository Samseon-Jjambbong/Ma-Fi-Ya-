using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks   
{
    public enum Panel { Login, Menu, Lobby, Room }

    [SerializeField] LoginPanel loginPanel;
    [SerializeField] MainPanel menuPanel;
    [SerializeField] RoomPanel roomPanel;
    [SerializeField] LobbyPanel lobbyPanel;

    private ClientState state;

    private void Update()
    {
        ClientState curState = PhotonNetwork.NetworkClientState;
        if (state == curState)
        {
            return;
        }

        state = curState;
        Debug.Log(state);
    }

    public override void OnConnected()
    {
        // 접속이 됐을 때 반응 구현
        SetActivePanel(Panel.Menu);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 접속이 안 됐을 때 반응 구현

        if (cause == DisconnectCause.ApplicationQuit)
        {
            return;
        }
        
        SetActivePanel(Panel.Login);
    }

    public override void OnCreatedRoom()
    {
        // 방 만들기 성공
        Debug.Log($"Creat room success");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // 방 만들기 실패한 경우
        Debug.Log($"Creat room failed with error : {message}({returnCode})");
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join random room failed with error : {message}({returnCode})");
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(Panel.Menu);
    }

    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Menu);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.PlayerEnterRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.PlayerleftRoom(otherPlayer);   
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomPanel.MasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        // 실시간으로 빠르게 바뀌지는 않음 (서버에다가 변수를 저장해둔 경우이기 떄문에)
        // 이 방 안에 있는 Property가 변경되었을 때 호출

        roomPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    private void Start()
    {
        SetActivePanel(Panel.Login);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        roomPanel.gameObject.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
}
