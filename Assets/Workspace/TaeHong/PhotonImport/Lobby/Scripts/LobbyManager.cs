using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby, Room }

    [SerializeField] LoginSystem.LoginManager loginPanel;
    [SerializeField] MainPanel menuPanel;
    [SerializeField] RoomPanel roomPanel;
    [SerializeField] LobbyPanel lobbyPanel;

    private ClientState state;
    
    private void Start()
    {
        SetActivePanel(Panel.Menu);
    }

    private void Update()
    {
        ClientState curState = PhotonNetwork.NetworkClientState;
        if (state == curState)
            return;

        state = curState;
        Debug.Log(state);
    }

    public override void OnConnected()
    {
        SetActivePanel(Panel.Menu);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Create room success");
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.PlayerEnterRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.PlayerLeftRoom(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomPanel.MasterClientSwitched(newMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Create room failed with error : {message}({returnCode})");
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.ApplicationQuit)
            return;

        if(cause == DisconnectCause.None )
            return;

        Debug.Log($"OnDisconnected : {cause}");
        VCamController.Instance.SetVCam(VCamController.VCam.Login);
        SetActivePanel(Panel.Login);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        roomPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        roomPanel.gameObject.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
}
