using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreator : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] int radius;    // 21이 가장 이상적

    private void Start()
    {
        CreatePlayers();
    }

    private void CreatePlayers()
    {
        int angle = 180 / ( Manager.Mafia.PlayerCount - 1 );    // 각 집의 간격의 각도

        int currentAngle = 0;
        for ( int i = 0; i < Manager.Mafia.PlayerCount; i++ )
        {
            Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, 1.8f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);
            Transform player = Instantiate(playerPrefab).transform;
            player.position = pos;

            Quaternion look = Quaternion.LookRotation(pos); // 센터를 바라보도록 rotation 조절
            player.rotation = look;

            currentAngle += angle;
        }
    }
}
