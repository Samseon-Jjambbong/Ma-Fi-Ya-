using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Mafia
{
    public class SharedData : MonoBehaviourPun
    {
        // Player State
        public bool[] deadPlayers;
        public bool[] blockedPlayers;
        public bool[] killedPlayers;
        public bool[] healedPlayers;

        // Player Actions
        public Dictionary<int, MafiaAction> sentActionDic = new Dictionary<int, MafiaAction>();
        public Dictionary<int, List<MafiaActionType>> receivedActionDic = new Dictionary<int, List<MafiaActionType>>();

        // Vote Info
        public int playerToKick = -1;

        // Game Loop
        public int clientFinishedCount;

        private void Start()
        {
            int playerCount = PhotonNetwork.CurrentRoom.Players.Count;
            deadPlayers = new bool[playerCount];
            blockedPlayers = new bool[playerCount];
            killedPlayers = new bool[playerCount];
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

        /******************************************************
        *                    Player Actions
        ******************************************************/
        #region Player Actions
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
        #endregion

        /******************************************************
        *                    Player States
        ******************************************************/
        #region Player States
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
        public void SetKilled(int idx, bool killed)
        {
            killedPlayers[idx] = killed;
        }

        public List<int> GetKilledPlayers()
        {
            List<int> killed = new List<int>();
            for(int i = 0; i < killedPlayers.Length; i++)
            {
                if (killedPlayers[i] == true)
                {
                    killed.Add(i + 1);
                }
            }
            return killed;
        }

        [PunRPC]
        public void SetDead(int idx, bool dead)
        {
            deadPlayers[idx] = dead;
        }

        [PunRPC]
        public void ResetPlayerStates()
        {
            int playerCount = PhotonNetwork.CurrentRoom.Players.Count;
            blockedPlayers = new bool[playerCount];
            healedPlayers = new bool[playerCount];
        }
        #endregion

        /******************************************************
        *                    Vote Info
        ******************************************************/
        #region Vote Info
        [PunRPC]
        public void SetPlayerToKick(int id)
        {
            playerToKick = id;
        }
        #endregion

        /******************************************************
        *                    Game Loop
        ******************************************************/
        #region Game Loop
        [PunRPC] // Call before callbacks
        public void ResetClientFinishedCount()
        {
            clientFinishedCount = 0;
        }

        [PunRPC]
        public void ClientFinished()
        {
            clientFinishedCount++;
        }
        #endregion
    }
}

