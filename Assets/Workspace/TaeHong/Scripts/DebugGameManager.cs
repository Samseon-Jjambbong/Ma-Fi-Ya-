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
        [SerializeField] int radius;    // 21이 가장 이상적
        
        private List<House> houses;
        [SerializeField] private int playerCount = 4;
        [SerializeField] private GameObject housePrefab;
        
        private void Start()
        {
            PhotonNetwork.LocalPlayer.NickName = $"TestPlayer {Random.Range(1000, 10000)}";
            PhotonNetwork.ConnectUsingSettings();
        }
        
        private void SpawnHouses()
        {
            houses = new List<House>();
        
            int angle = 180 / (playerCount - 1);    // 각 집의 간격의 각도

            int currentAngle = 0;
            for (int i = 0; i < playerCount; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, 1.8f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);
                House house = Instantiate(housePrefab, pos, Quaternion.LookRotation(pos)).GetComponent<House>();

                currentAngle += angle;
            
                houses.Add(house);
            }
        }

        // private void SpawnPlayers()
        // {
        //     int angle = 180 / ( playerCount - 1 );    // 각 플레이어의 간격의 각도
        //     for ( int i = 0; i < playerCount; i++ )
        //     {
        //         int currentAngle = 180 - angle * i;
        //
        //         // 순번에 맞는 플레이어의 위치 설정
        //         Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * (radius - 6), 2.22f,
        //             Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);
        //         // PhotonNetwork.Instantiate를 통해 각 플레이어 캐릭터 생성, 센터를 바라보도록 rotation 설정
        //         Transform player = PhotonNetwork.Instantiate("TestPlayer", pos, Quaternion.LookRotation(pos)).transform;
        //     }
        // }
        
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

        private void RandomizeRoles()
        {
            // TODO: Implement this
        }
        
        private void DebugGameStart()
        {
            // Master Client Responsibilities
            if ( PhotonNetwork.IsMasterClient )
            {
                RandomizeRoles(); // Decide which roles each player shoudl
                SpawnHouses(); // Spawn {PlayerCount} Houses
                //SpawnPlayers();
                SpawnPlayer();
            }
        }

        private void SpawnPlayer()
        {
            // Spawn Player in front of respective house
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            
            House house = houses[PhotonNetwork.LocalPlayer.ActorNumber - 1]; // TODO: Fix this
            Vector3 spawnPos = house.transform.position + house.transform.forward * -5;
            Quaternion spawnRot = Quaternion.LookRotation(spawnPos);
            
            PhotonNetwork.Instantiate("Player", spawnPos, spawnRot, 0);
        }
    }
}