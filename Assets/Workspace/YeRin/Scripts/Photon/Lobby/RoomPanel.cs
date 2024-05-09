using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] Button startButton;

    private List<PlayerEntry> playerList;

    private void Awake()
    {
        playerList = new List<PlayerEntry>();
    }

    private void OnEnable()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.SetPlayer(player);
            playerList.Add(playerEntry);
        }

        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
        AllPlayerReadyCheck();
        PhotonNetwork.AutomaticallySyncScene = true;    // 이게 true가 아닐 경우 한 명이 씬 전환을 해도 따라가지 않음
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
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        playerEntry.SetPlayer(newPlayer);
        playerList.Add(playerEntry);

        AllPlayerReadyCheck();
    }

    public void PlayerleftRoom(Player otherPlayer)
    {
        PlayerEntry playerEntry = null;

        foreach (PlayerEntry entry in playerList) 
        {
            if (entry.Player.ActorNumber == otherPlayer.ActorNumber)    // id로 비교
            {
                playerEntry = entry;
            }
        }

        // 위에 있는 것과 같은 내용
        // PlayerEntry find = playerList.Find(entry => entry.Player == otherPlayer);

        playerList.Remove(playerEntry);
        Destroy(playerEntry.gameObject);

        AllPlayerReadyCheck();
    }

    public void MasterClientSwitched(Player newMasterClient)
    {
        startButton.gameObject.SetActive(newMasterClient.IsLocal);    // 새로운 방장이 나면

        AllPlayerReadyCheck();
    }
    public void PlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        PlayerEntry playerEntry = null;
        foreach (PlayerEntry entry in playerList) 
        {
            if (entry.Player.ActorNumber == targetPlayer.ActorNumber) 
            {
                playerEntry = entry;
            }
        }
        playerEntry.ChangeCustomProperty(changedProps);

        AllPlayerReadyCheck();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void AllPlayerReadyCheck()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        int readyCount = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady())
            {
                readyCount++;
            }
        }

        startButton.interactable = readyCount == PhotonNetwork.PlayerList.Length;
    }
}
