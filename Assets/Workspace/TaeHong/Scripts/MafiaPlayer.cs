using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Tae
{
    public class MafiaPlayer : MonoBehaviour
    {
        private MafiaRole role;
        private MafiaActionPQ actionsOnThisPlayer = new MafiaActionPQ();

        private void Start()
        {
            // 플레이어 역할 받기
            role = PhotonNetwork.LocalPlayer.GetPlayerRole();
        }


    }
}
