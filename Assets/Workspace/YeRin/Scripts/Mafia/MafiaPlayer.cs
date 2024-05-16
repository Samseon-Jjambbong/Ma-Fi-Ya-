using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using TMPro;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Class for Mafia mode players
/// </summary>
public class MafiaPlayer : MonoBehaviourPun
{
    [SerializeField] TMP_Text nickNameText;

    // 플레이어의 생존 여부
    private bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }

    private Dictionary<int, Player> playerDic;

    // Tae Player Logic
    public MafiaActionType actionType;
    public int targetPlayerID;
    public MafiaActionPQ actionsOnThisPlayer = new MafiaActionPQ();

    protected virtual void Start()
    {
        // 플레이어 역할 받기
        playerDic = PhotonNetwork.CurrentRoom.Players;
    }

    public MafiaRole GetRole()
    {
        return PhotonNetwork.LocalPlayer.GetPlayerRole();
    }

    [PunRPC]
    public void ShowNightResults()
    {
        Debug.Log($"Player{PhotonNetwork.LocalPlayer.ActorNumber} Results. Count: {actionsOnThisPlayer.Count}");
        while(actionsOnThisPlayer.Count > 0)
        {
            MafiaAction action = actionsOnThisPlayer.Dequeue();
            Debug.Log($"Action on this player: {action.actionType}");
        }
    }

    #region Game Logic
    [PunRPC]
    public void OnChooseTarget(int[] serialized)
    {
        // Deserialize Action
        MafiaAction action = new MafiaAction(serialized);
        Debug.Log($"sender: {action.sender}");
        Debug.Log($"receiver: {action.receiver}");
        Debug.Log($"action: {action.actionType}");

        if(PhotonNetwork.LocalPlayer.ActorNumber == action.sender)
        {
            targetPlayerID = action.receiver;
            Debug.Log($"{GetRole()} targeted Player{targetPlayerID}");
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber == action.receiver)
        {
            actionsOnThisPlayer.Enqueue(action);
            Debug.Log($"{action.receiver} got {action.actionType}ed");
        }
    }
    #endregion

    #region Photon
    public void SetPlayerHouse( int playerNumber )
    {
        photonView.RPC("AddHouseList", RpcTarget.All, playerNumber);
        Manager.Mafia.Houses [playerNumber].ActivateOutline(false);
    }

    [PunRPC]
    private void AddHouseList( int playerNumber )
    {
        Manager.Mafia.Houses [playerNumber].HouseOwner = this;
    }

    public void SetNickName(string nickName)
    {
        photonView.RPC("NickName", RpcTarget.All, nickName);
    }

    [PunRPC]
    private void NickName( string nickName )
    {
        nickNameText.text = nickName;
    }
    #endregion
}