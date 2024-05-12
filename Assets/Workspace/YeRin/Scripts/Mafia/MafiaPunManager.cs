using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using Photon.Pun.UtilityScripts;

public class MafiaPunManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float CountDownTime;

    private Dictionary<int, Player> playerDic;

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
        CreatePlayer();
    }

    [SerializeField] int radius;

    private void CreatePlayer()
    {
        int angle = 180 / ( Manager.Mafia.PlayerCount - 1 );    // 각 플레이어의 간격의 각도

        int playerNumber = -1;

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

        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius);
        Transform player = PhotonNetwork.Instantiate("TestPlayer", pos, Quaternion.identity).transform;

        Quaternion look = Quaternion.LookRotation(pos); // 센터를 바라보도록 rotation 조절
        player.rotation = look;
    }
}

