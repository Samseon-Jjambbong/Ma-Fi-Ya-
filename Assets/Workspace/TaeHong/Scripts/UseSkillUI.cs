using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseSkillUI : BaseUI
{
    [SerializeField] MafiaRoleDataSO dataSO;

    private void OnEnable()
    {
        MafiaRole playerRole = PhotonNetwork.LocalPlayer.GetPlayerRole();
        MafiaRoleData data = dataSO.GetData(playerRole);

        GetUI<Image>("IconButton").sprite = data.roleIcon;
    }
}
