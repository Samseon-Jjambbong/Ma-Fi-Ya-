using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using Photon.Pun.UtilityScripts;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MafiaPunManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float countDownTime;
    [SerializeField] private MafiaGameFlow gameFlow;
    [SerializeField] private GameObject housePrefab;
    
    [SerializeField] int playerRadius;
    [SerializeField] int houseRadius;
    private Dictionary<int, Player> playerDic;
    private bool isDay;
    
    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoaded(true);
        playerDic = PhotonNetwork.CurrentRoom.Players;
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
        SpawnHouses();

        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();
        while ( PhotonNetwork.Time - loadTime < countDownTime )
        {
            int remainTime = ( int ) ( countDownTime - ( PhotonNetwork.Time - loadTime ) );
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
        CreatePlayer();
        //gameFlow.StartGameFlow();
        Manager.Mafia.StartGame();
    }

    private void CreatePlayer()
    {
        int angle = 180 / ( Manager.Mafia.PlayerCount - 1 );    // 각 플레이어의 간격의 각도

        int playerNumber = -1;

        // 플레이어의 게임 입장 순번 찾아내기
        for (int i = 1; i <= playerDic.Count; i++)
        {
            if ( playerDic [i] == PhotonNetwork.LocalPlayer)
            {
                playerNumber = i - 1;
            }
        }

        if (playerNumber == -1)
        {
            Debug.Log("Can't found LocalPlayer Number");
            return;
        }

        int currentAngle = 180 - angle * playerNumber;

        // 순번에 맞는 플레이어의 위치 설정
        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * playerRadius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * playerRadius);
        // PhotonNetwork.Instantiate를 통해 각 플레이어 캐릭터 생성, 센터를 바라보도록 rotation 설정
        Transform player = PhotonNetwork.Instantiate("TestPlayer", pos, Quaternion.LookRotation(pos)).transform;
    }

    private void SpawnHouses()
    {
        int angle = 180 / ( Manager.Mafia.PlayerCount - 1 );    // 각 집의 간격의 각도

        int currentAngle = 180;
        for ( int i = 0; i < Manager.Mafia.PlayerCount; i++ )
        {
            Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * houseRadius, 1.8f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * houseRadius);
            House house = Instantiate(housePrefab, pos, Quaternion.LookRotation(pos)).GetComponent<House>();
            
            Manager.Mafia.Houses.Add(house);
            currentAngle -= angle;
        }
    }
    
    
}
