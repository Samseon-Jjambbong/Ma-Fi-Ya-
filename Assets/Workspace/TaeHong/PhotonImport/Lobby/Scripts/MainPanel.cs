using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameMode { Mafia, Knife, HideAndSeek }
public class MainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Dropdown maxPlayerDropdown;
    [SerializeField] ToggleGroup gameModeToggleGroup;

    private void OnEnable()
    {
        createRoomPanel.SetActive(false);
    }

    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
    }

    public void CreateRoomConfirm()
    {
        // Get room option inputs
        string roomName = roomNameInputField.text == "" ? $"Room {Random.Range(1000, 10000)}" : roomNameInputField.text;
        int maxPlayers = int.Parse(maxPlayerDropdown.options [maxPlayerDropdown.value].text);
        GameMode gameMode = gameModeToggleGroup.GetFirstActiveToggle().GetComponent<GameModeButton>().gameMode;
        
        // Create and join new room
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        options.SetGameMode(gameMode, true);
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);
    }

    public void RandomMatching()
    {
        // Join random room. If there aren't any rooms available, create default room and join.
        string name = $"Room {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 8 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName:name, roomOptions:options);
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void Logout()
    {
        PhotonNetwork.Disconnect();
    }
}
