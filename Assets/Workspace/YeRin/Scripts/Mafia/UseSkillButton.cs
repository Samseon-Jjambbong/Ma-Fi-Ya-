using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// programmer : Yerin
/// 
/// </summary>
public class UseSkillButton : MonoBehaviourPun
{
    [SerializeField] House house;
 
    public void SetTarget()
    {
        NightMafiaMove [] players =  FindObjectsOfType<NightMafiaMove>();

        foreach ( NightMafiaMove player in players)
        {
            if ( player.IsActive )
            {
                player.Target = house.gameObject;
                player.MoveToTarget();
            }
        }
    }
}
