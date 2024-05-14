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

    protected MafiaRole role;
    protected MafiaActionType actionType;
    private MafiaActionPQ actionsToShow = new MafiaActionPQ();

    protected virtual void Start()
    {
        // 플레이어 역할 받기
        role = PhotonNetwork.LocalPlayer.GetPlayerRole();
        playerDic = PhotonNetwork.CurrentRoom.Players;
    }

    #region Game Logic
    private void OnEnable()
    {
        Manager.Event.pairEventDic["useSkill"].OnEventRaised += UseSkill;
    }

    protected virtual void UseSkill((int, int) info)
    {
        //MafiaAction action = new MafiaAction(info.Item1, info.Item2, mafiaActionType);
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