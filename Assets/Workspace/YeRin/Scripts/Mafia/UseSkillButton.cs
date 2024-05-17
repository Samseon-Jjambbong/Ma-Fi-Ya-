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


        GameObject obj = Instantiate(Manager.Mafia.NightMafia, Manager.Mafia.NightMafiaPos, Manager.Mafia.NightMafia.transform.rotation);

        foreach (MafiaPlayer player in FindObjectsOfType<MafiaPlayer>())
        {
            if (player.IsMine)
            {
                obj.GetComponentInChildren<Renderer>().material.color = player.GetComponentInChildren<Renderer>().material.color;
            }
        }
        NightMafiaMove mafia = obj.GetComponent<NightMafiaMove>();

        mafia.Target = house.gameObject;
        house.VisitorId(PhotonNetwork.LocalPlayer.ActorNumber);
        mafia.MoveToTarget();

        house.MafiaComesHome();
    }
}
