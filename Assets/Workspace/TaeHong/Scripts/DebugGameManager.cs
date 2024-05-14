using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public struct MafiaAction
{
    private int sender;
    private int receiver;
    private MafiaActionType actionType;

    public MafiaAction(int sender, int receiver, MafiaActionType action)
    {
        this.sender = sender;
        this.receiver = receiver;
        this.actionType = action;
    }

    public int GetActionPrio()
    {
        return (int) actionType;
    }
}

public class MafiaActionPQ
{
    List<MafiaAction> actions;

    public void Enqueue(MafiaAction action)
    {
        actions.Add(action);
    }

    public MafiaAction Dequeue()
    {
        int prioIndex = 0;
        for(int i = 0; i < actions.Count; i++)
        {
            if(actions[i].GetActionPrio() < actions[prioIndex].GetActionPrio())
            {
                prioIndex = i;
            }
        }
        MafiaAction action = actions[prioIndex];
        actions.RemoveAt(prioIndex);
        return action;
    }

    public MafiaAction Peek()
    {
        int prioIndex = 0;
        for(int i = 0; i < actions.Count; i++)
        {
            if(actions[i].GetActionPrio() < actions[prioIndex].GetActionPrio())
            {
                prioIndex = i;
            }
        }
        return actions[prioIndex];
    }
}

namespace Tae
{
    public class DebugGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string debugRoomName;
        [SerializeField] int radius;    // 21이 가장 이상적
        
        private List<House> houses;
        [SerializeField] private int playerCount = 4;
        [SerializeField] private GameObject housePrefab;

        [SerializeField] MafiaRolesSO mafiaRolesSO;
        private MafiaRole[] roles;
        private MafiaGame game = new MafiaGame();
        
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

        private void RandomizeRoles(int numPlayers)
        {
            // Get role list
            roles = mafiaRolesSO.GetRoles(numPlayers);

            // Shuffle list algorithm
            int n = roles.Length;
            for ( int i = n - 1; i > 0; i-- )
            {
                // Generate a random index j such that 0 <= j <= i
                int j = Random.Range(0, i + 1);

                // Swap array[i] with array[j]
                var temp = roles[i];
                roles[i] = roles[j];
                roles[j] = temp;
            }
        }

        private void AssignRole()
        {
            MafiaRole role = roles[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            Debug.Log(role);
            PhotonNetwork.LocalPlayer.SetPlayerRole(role);
            game.AddPlayer(role);
        }
        
        private void DebugGameStart()
        {
            // Master Client Responsibilities
            if ( PhotonNetwork.IsMasterClient )
            {
                //RandomizeRoles(PhotonNetwork.CurrentRoom.PlayerCount); // Create and randomize roles
                RandomizeRoles(4);
                AssignRole();

                SpawnHouses(); // Spawn {PlayerCount} Houses
                //SpawnPlayers();
                SpawnPlayer();

                GetComponent<MafiaGameFlow>().TestGameFlow();
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

    public class MafiaGame
    {
        private int numMafias;
        private int numCivilians;

        public void AddPlayer(MafiaRole role)
        {
            if(role == MafiaRole.Mafia)
                numMafias++;
            numCivilians++;
        }

        public bool RemovePlayer(MafiaRole removedRole) // True == Civilian Win, False == Mafia Win
        {
            if(removedRole == MafiaRole.Mafia)
            {
                numMafias--;
                if(numMafias == 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                numCivilians--;
                if(numMafias == numCivilians)
                {
                    return false;
                }
                return true;
            }
        }
    }
}

