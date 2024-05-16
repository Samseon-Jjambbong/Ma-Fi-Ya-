using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using Photon.Pun.UtilityScripts;
using UnityEngine.InputSystem;

public class MafiaPunManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float CountDownTime;

    [SerializeField] int palyerRadius;
    [SerializeField] int houseRadius;
    [SerializeField] List<Color> colorList;

    private Dictionary<int, Player> playerDic;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoaded(true);
        playerDic = PhotonNetwork.CurrentRoom.Players;

        for (int i = 0; i < playerDic.Count; i++) 
        {
            colorList.Add(new Color(Random.value, Random.value, Random.value, 1f));
        }
    }

    public override void OnPlayerPropertiesUpdate( Player targetPlayer, PhotonHashTable changedProps )
    {
        if ( changedProps.ContainsKey(CustomProperty.LOAD) )
        {
            infoText.text = $"{PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            if ( PlayerLoadCount() == PhotonNetwork.PlayerList.Length )
            {
                if ( PhotonNetwork.IsMasterClient )
                {
                    PhotonNetwork.CurrentRoom.SetGameStart(true);
                    PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
                }
            }
            else
            {
                infoText.text = $"Wait {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            }
        }
    }

    public override void OnRoomPropertiesUpdate( PhotonHashTable propertiesThatChanged )
    {
        if ( propertiesThatChanged.ContainsKey(CustomProperty.GAMESTARTTIME) )
        {
            StartCoroutine(StartTime());
        }
    }

    IEnumerator StartTime()
    {
        if ( PhotonNetwork.IsMasterClient )
        {
            SpawnHouses(); // Spawn {PlayerCount} Houses
        }

        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();
        while ( PhotonNetwork.Time - loadTime < CountDownTime )
        {
            int remainTime = ( int ) ( CountDownTime - ( PhotonNetwork.Time - loadTime ) );
            infoText.text = ( remainTime + 1 ).ToString();
            yield return null;
        }

        infoText.text = "Game Start";
        GameStart();
        yield return new WaitForSeconds(3f);

        infoText.text = "";
    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach ( Player player in PhotonNetwork.PlayerList )
        {
            if ( player.GetLoaded() )
            {
                loadCount++;
            }
        }

        return loadCount;
    }

    public void GameStart()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        int angle = 180 / ( Manager.Mafia.PlayerCount - 1 );    // 각 플레이어의 간격의 각도

        int playerNumber = -1;

        // 플레이어의 게임 입장 순번 찾아내기
        for (int i = 1; i <= playerDic.Count; i++)
        {
            if ( playerDic [i] == PhotonNetwork.LocalPlayer)
            {
                Debug.Log($"i: {i}");
                playerNumber = i - 1;
            }
        }

        if (playerNumber == -1)
        {
            Debug.Log("Can't found LocalPlayer Number");
            return;
        }

        int currentAngle = 180 - angle * playerNumber;

        if (playerNumber == playerDic.Count - 1)
        {
            currentAngle = 0;
        }

        // 순번에 맞는 플레이어의 위치 설정
        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * palyerRadius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * palyerRadius);
        // PhotonNetwork.Instantiate를 통해 각 플레이어 캐릭터 생성, 센터를 바라보도록 rotation 설정
        GameObject player = PhotonNetwork.Instantiate("Mafia", pos, Quaternion.LookRotation(-pos));
        player.GetComponent<MafiaPlayer>().SetPlayerHouse(playerNumber);
        player.GetComponent<MafiaPlayer>().SetNickName(PhotonNetwork.PlayerList [playerNumber].NickName);
    }

    private void SpawnHouses()
    {
        int angle = 180 / ( Manager.Mafia.PlayerCount - 1 );    // 각 집의 간격의 각도

        int currentAngle = 180;
        for ( int i = 0; i < Manager.Mafia.PlayerCount; i++ )
        {
            if (i == Manager.Mafia.PlayerCount - 1)
            {
                currentAngle = 0;
            }

            Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * houseRadius, 1.8f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * houseRadius);
            GameObject houseGO = PhotonNetwork.InstantiateRoomObject("House", pos, Quaternion.LookRotation(pos));

            currentAngle -= angle;
        }
    }
}

