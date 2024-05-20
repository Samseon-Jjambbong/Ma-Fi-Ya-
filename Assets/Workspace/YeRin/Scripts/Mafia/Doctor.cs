using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Doctor : MafiaPlayer
{
    protected override void Start()
    {
        base.Start();

        actionType = MafiaActionType.Heal;
    }
}