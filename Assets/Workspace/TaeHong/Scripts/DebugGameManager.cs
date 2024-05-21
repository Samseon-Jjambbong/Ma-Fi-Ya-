using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
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
        [SerializeField] int radius = 21;

        [Header("Night Mafia Move")]
        [SerializeField] NightMafiaMove mafia;
        [SerializeField] GameObject mafiaPrefab;
        [SerializeField] Vector3 mafiaSpawnPoint;
        [SerializeField] private List<House> houses;
        [SerializeField] private int playerCount = 4;
        [SerializeField] private GameObject housePrefab;
        [SerializeField] AnimationFactory animFactory;
        [SerializeField] Spring spring;

        private void Start()
        {
            SpawnHouses();
            SpawnNightMafia(0);
            ShowPlayerKicked();
        }

        public void SpawnNightMafia(int houseIdx)
        {
            StartCoroutine(animFactory.PlayerGoActionRoutine(new MafiaAction(2, 3, (MafiaActionType)1)));
            StartCoroutine(animFactory.PlayerComeActionRoutine(1, (MafiaActionType) 1));
        }

        public void ShowPlayerDeath(int houseIdx)
        {
            StartCoroutine(animFactory.SpawnPlayerDie(houses[houseIdx]));
        }

        public void ShowPlayerKicked()
        {
            StartCoroutine(animFactory.PlayerKickedActionRoutine(houses[1]));

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
            Manager.Mafia.Houses = houses;
        }
    }
}

