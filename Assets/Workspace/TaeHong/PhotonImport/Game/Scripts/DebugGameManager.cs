
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebugGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string debugRoomName;
    [SerializeField] private TMP_Text infoText;

    [SerializeField] private float spawnStoneTime = 3f;
    
    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"TestPlayer {Random.Range(1000, 10000)}";
        infoText.text = "Debug Mode";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;
        TypedLobby typedLobby = new TypedLobby("DebugLobby", LobbyType.Default);
        PhotonNetwork.JoinOrCreateRoom(debugRoomName, options, typedLobby);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        StartCoroutine(GameStartDelay());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            spawnStoneRoutine = StartCoroutine(SpawnStoneRoutine());
        }
    }

    IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(1);
        GameStart();
    }

    public void GameStart()
    {
        Vector2 spawnPos = Random.insideUnitCircle * 30;
        PhotonNetwork.Instantiate("Player", new Vector3(spawnPos.x, 0,spawnPos.y), Quaternion.identity);

        if (PhotonNetwork.IsMasterClient)
        {
            spawnStoneRoutine = StartCoroutine(SpawnStoneRoutine());
        }
    }

    Coroutine spawnStoneRoutine;
    private IEnumerator SpawnStoneRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnStoneTime);

            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector3 position = new Vector3(direction.x, 0, direction.y) * 200f;

            Vector3 force = -position.normalized * 30f + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            Vector3 torque = Random.insideUnitSphere * Random.Range(1f, 3f);
            object[] instantiateData = { force, torque };

            if (Random.Range(0, 2) < 1)
            {
                PhotonNetwork.InstantiateRoomObject("LargeStone", position, Random.rotation, 0, instantiateData);
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject("SmallStone", position, Random.rotation, 0, instantiateData);
            }
        }
    }
}
