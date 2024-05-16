using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] Button startButton;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    
    private List<PlayerEntry> playerList;

    private void Awake()
    {
        playerList = new List<PlayerEntry>();
    }

    private void OnEnable()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.UpdateInfo(player);
            playerList.Add(playerEntry);
        }
        
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoaded(false);
        AllPlayersReadyCheck();
        
        // Follow room owner on scene change (game start)
        PhotonNetwork.AutomaticallySyncScene = true;
        
        // Display GameMode
        string gameMode = Enum.GetName(typeof(GameMode), PhotonNetwork.CurrentRoom.GetGameMode());
        gameModeText.text = $"Game Mode: {gameMode}";
        
        // Display PlayerCount
        playerCountText.text =
            $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    private void OnDisable()
    {
        for (int i = 0; i < playerContent.childCount; i++)
        {
            Destroy(playerContent.GetChild(i).gameObject);
        }
        playerList.Clear();
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("TestGame");
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        playerEntry.UpdateInfo(newPlayer);
        playerList.Add(playerEntry);
        
        AllPlayersReadyCheck();
        
        // Display PlayerCount
        playerCountText.text =
            $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void PlayerLeftRoom(Player otherPlayer)
    {
        PlayerEntry playerEntry = null;
        foreach (var entry in playerList)
        {
            if (entry.Player.ActorNumber == otherPlayer.ActorNumber)
            {
                playerEntry = entry;
            }
        }
        
        Destroy(playerEntry.gameObject);
        playerList.Remove(playerEntry);

        playerCountText.text =
            $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        AllPlayersReadyCheck();
    }

    public void MasterClientSwitched(Player newMasterClient)
    {
        startButton.gameObject.SetActive(newMasterClient.IsLocal);
        AllPlayersReadyCheck();
    }

    public void PlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        PlayerEntry playerEntry = null;
        foreach (var entry in playerList)
        {
            if (entry.Player.ActorNumber == targetPlayer.ActorNumber)
            {
                playerEntry = entry;
            }
        }
        playerEntry?.ChangeCustomProperties(changedProps);
        AllPlayersReadyCheck();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void AllPlayersReadyCheck()
    {
        // Only room owner needs to check if everyone is ready
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Count how many players are ready
        int readyCount = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady())
            {
                readyCount++;
            }
        }
        
        // If everyone is ready, owner can start game
        startButton.interactable = (readyCount == PhotonNetwork.PlayerList.Length);
    }
}
