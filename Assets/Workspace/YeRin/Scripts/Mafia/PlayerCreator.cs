using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreator : MonoBehaviourPun// , IPunObservable
{
    [SerializeField] int radius;

    private void CreatePlayer()
    {
        int angle = 180 / ( Manager.Mafia.PlayerCount - 1 );    // 각 플레이어의 간격의 각도

        int playerNumber = photonView.Owner.GetPlayerNumber();

        int currentAngle = 180 - angle * playerNumber;

        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);
        Transform player = PhotonNetwork.Instantiate("TestPlayer", pos, Quaternion.identity).transform;

        Quaternion look = Quaternion.LookRotation(pos); // 센터를 바라보도록 rotation 조절
        player.rotation = look;
        /*for ( int i = 0; i < PhotonNetwork.CountOfPlayers; i++ )
        {
            Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);
            Transform player = PhotonNetwork.Instantiate("TestPlayer", pos, Quaternion.identity).transform;

            Quaternion look = Quaternion.LookRotation(pos); // 센터를 바라보도록 rotation 조절
            player.rotation = look;

            currentAngle -= angle;
        }*/
    }

    /*public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        throw new System.NotImplementedException();
    }*/
}
