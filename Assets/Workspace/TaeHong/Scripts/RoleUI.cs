using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class RoleUI : BaseUI
{
    [SerializeField] MafiaRoleDataSO dataSO;

    private void OnEnable()
    {
        MafiaRole playerRole = PhotonNetwork.LocalPlayer.GetPlayerRole();
        MafiaRoleData data = dataSO.GetData(playerRole);

        GetUI<Image>("RoleIcon").sprite = data.roleIcon;
        GetUI<TextMeshProUGUI>("RoleName").text = $"Your Role Is : {data.roleName}";
    }
}
