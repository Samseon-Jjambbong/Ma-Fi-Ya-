using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tae
{
    public class DebugGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string debugRoomName;
        private HouseCreator houseCreator;
        [SerializeField] int radius;    // 21이 가장 이상적

        private List<House> houses;
        
        private void Start()
        {
            PhotonNetwork.LocalPlayer.NickName = $"TestPlayer {Random.Range(1000, 10000)}";
            PhotonNetwork.ConnectUsingSettings();
        }

        private void SpawnHouses()
        {
            if ( PhotonNetwork.IsMasterClient )
            {
                CreateHouses();
            }
        }
        
        private void CreateHouses()
        {
            int playerCount = 4;
            houses = new List<House>();
        
            int angle = 180 / (playerCount - 1);    // 각 집의 간격의 각도

            int currentAngle = 0;
            for (int i = 0; i < playerCount; i++)
            {
                GameObject houseGO = PhotonNetwork.Instantiate("House", Vector3.zero, Quaternion.identity);
                Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, 1.8f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);

                Transform house = houseGO.transform;
                house.position = pos;

                Quaternion look = Quaternion.LookRotation(-pos); // 센터를 바라보도록 rotation 조절
                house.rotation = look;

                currentAngle += angle;
            
                houses.Add(houseGO.GetComponent<House>());
            }
        }
        
        public override void OnConnectedToMaster()
        {
            RoomOptions options = new RoomOptions { MaxPlayers = 8, IsVisible = false };
            TypedLobby typedLobby = new TypedLobby("DebugLobby", LobbyType.Default);
            PhotonNetwork.JoinOrCreateRoom(debugRoomName, options, typedLobby);
        }
        public override void OnJoinedRoom()
        {
            StartCoroutine(DebugGameSetupDelay());
        }
        
        IEnumerator DebugGameSetupDelay()
        {
            yield return new WaitForSeconds(1f);
            DebugGameStart();
        }
        
        private void DebugGameStart()
        {
            // Spawn {PlayerCount} Houses
            SpawnHouses();
            
            // Spawn Player in front of respective house
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            
            House house = houses[PhotonNetwork.LocalPlayer.ActorNumber - 1]; // TODO: Fix this
            Vector3 spawnPos = house.transform.position + house.transform.forward * 5;
            Quaternion spawnRot = Quaternion.LookRotation(-spawnPos);
            
            PhotonNetwork.Instantiate("Player", spawnPos, spawnRot, 0);
        }
    }
}