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

    protected override void UseSkill( int targetPlayer )
    {
        base.UseSkill(targetPlayer);

        // Manager.Mafia.PlayerUsedSkill(player, targetPlayer, MafiaAction.Heal);
        photonView.RPC("Heal", RpcTarget.All, targetPlayer);
        CanUseSkill = false;
    }

    [PunRPC]
    private void Heal( MafiaPlayer targetPlayer )
    {
        targetPlayer.IsHealed = true;
    }
}