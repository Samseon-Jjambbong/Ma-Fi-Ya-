using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Doctor : MafiaPlayer
{
    private void Start()
    {
        IsMafia = false;
    }
    protected override void UseSkill( MafiaPlayer targetPlayer )
    {
        base.UseSkill(targetPlayer);

        photonView.RPC("Heal", RpcTarget.All, targetPlayer);
        CanUseSkill = false;
    }

    [PunRPC]
    private void Heal( MafiaPlayer targetPlayer )
    {
        targetPlayer.IsHealed = true;
    }
}