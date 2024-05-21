using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPostiotnTest : MonoBehaviour
{
    [SerializeField] GameObject playerprefab;

    [SerializeField] int maxPlayer;
    [SerializeField] int playerCount;
    [SerializeField] float playerRadius;

    // Start is called before the first frame update
    void OnEnable()
    {
        for(int i = 0; i < maxPlayer; i++)
        {
            playerCount++;
            SpawnPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPlayer()
    {
        int angle = 360 / (maxPlayer);    // 각 플레이어의 간격의 각도

        // 플레이어의 게임 입장 순번 찾아내기


        int currentAngle;
        if (playerCount == maxPlayer-1)
        {
            currentAngle = 0; // 마지막 플레이어일 때는 0도에 배치
        }
        else
        {
            currentAngle = 360 - (angle * (playerCount + 1)); // 다른 플레이어들은 360도에서 차례대로 각도 빼기
        }

        // 순번에 맞는 플레이어의 위치 설정
        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * playerRadius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * playerRadius);

        // PhotonNetwork.Instantiate를 통해 각 플레이어 캐릭터 생성, 센터를 바라보도록 rotation 설정
        GameObject player = Instantiate(playerprefab, pos, Quaternion.LookRotation(-pos));
        //색깔 설정
        //GameObject player = PhotonNetwork.Instantiate("Mafia", pos, Quaternion.LookRotation(-pos));
        //player.GetComponent<MafiaPlayer>().SetPlayerHouse(playerNumber);
        //player.GetComponent<MafiaPlayer>().SetNickName(PhotonNetwork.PlayerList[playerNumber].NickName);
    }

}
