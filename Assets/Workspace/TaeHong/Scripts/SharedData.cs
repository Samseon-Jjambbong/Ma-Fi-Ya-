using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Mafia
{
    public class SharedData : MonoBehaviourPun
    {
        // Player State
        public bool[] blockedPlayers;
        public bool[] deadPlayers;
        public bool[] healedPlayers;

        // Player Actions
        public Dictionary<int, MafiaAction> sentActionDic = new Dictionary<int, MafiaAction>();
        public Dictionary<int, List<MafiaActionType>> receivedActionDic = new Dictionary<int, List<MafiaActionType>>();

        private void Start()
        {
            int playerCount = PhotonNetwork.CurrentRoom.Players.Count;
            blockedPlayers = new bool[playerCount];
            deadPlayers = new bool[playerCount];
            healedPlayers = new bool[playerCount];
        }

        public int ActivePlayerCount()
        {
            int count = 0;
            foreach (bool dead in deadPlayers)
            {
                if (!dead)
                    count++;
            }
            return count;
        }

        [PunRPC]
        public void ResetPlayerStates()
        {
            int playerCount = PhotonNetwork.CurrentRoom.Players.Count;
            blockedPlayers = new bool[playerCount];
            healedPlayers = new bool[playerCount];
        }

        [PunRPC]
        public void SetBlocked(int idx, bool blocked)
        {
            blockedPlayers[idx] = blocked;
        }

        [PunRPC]
        public void SetHealed(int idx, bool healed)
        {
            healedPlayers[idx] = healed;
        }

        [PunRPC]
        public void SetDead(int idx, bool dead)
        {
            deadPlayers[idx] = dead;
        }

        [PunRPC]
        public void AddAction(int[] serialized)
        {
            MafiaAction action = new MafiaAction(serialized);
            Debug.Log($"Add Action RPC Info to SharedData: {action.sender} {action.receiver} {action.actionType}");
            sentActionDic.Add(action.sender, action);
            if (!receivedActionDic.ContainsKey(action.receiver))
            {
                List<MafiaActionType> list = new List<MafiaActionType> { action.actionType };
                receivedActionDic.Add(action.receiver, list);
            }
            else
            {
                receivedActionDic[action.receiver].Add(action.actionType);
            }
        }

        [PunRPC]
        public void ClearActionInfo()
        {
            sentActionDic.Clear();
            receivedActionDic.Clear();
        }
    }
}

