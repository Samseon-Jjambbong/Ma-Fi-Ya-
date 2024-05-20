using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text gameMode;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] Button joinRoomButton;

    private RoomInfo roomInfo;

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }

    public void UpdateInfo(RoomInfo info)
    {
        roomInfo = info;
        roomName.text = roomInfo.Name;
        gameMode.text = Enum.GetName(typeof(GameMode),(GameMode) roomInfo.CustomProperties["GameMode"]);
        currentPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        joinRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;
    }
}
