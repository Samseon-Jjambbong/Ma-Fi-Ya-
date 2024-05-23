using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;

public class RoleUI : BaseUI
{
    [SerializeField] MafiaRoleDataSO dataSO;

    public void InitBegin()
    {
        MafiaRole playerRole = PhotonNetwork.LocalPlayer.GetPlayerRole();
        MafiaRoleData data;

        // If insane, don't reveal
        if(playerRole == MafiaRole.Insane)
        {
            data = dataSO.GetData(Manager.Mafia.Player.actionType);
        }
        else
        {
            data = dataSO.GetData(playerRole);
        }

        GetUI<Image>("RoleIcon").sprite = data.roleIcon;
        GetUI<TextMeshProUGUI>("RoleName").text = data.roleName;
    }

    public void InitDead(int playerID)
    {
        MafiaRole playerRole = PhotonNetwork.CurrentRoom.Players[playerID].GetPlayerRole();
        MafiaRoleData data = dataSO.GetData(playerRole);

        GetUI<TextMeshProUGUI>("Text Top").text = $"{PhotonNetwork.CurrentRoom.Players[playerID].NickName}의 직업은";
        GetUI<Image>("RoleIcon").sprite = data.roleIcon;
        GetUI<TextMeshProUGUI>("RoleName").text = data.roleName;
    }
}
