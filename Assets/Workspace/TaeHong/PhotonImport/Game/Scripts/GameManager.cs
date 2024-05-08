using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private float countDownTime;
    
    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoaded(true);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
            {
                // Everyone finished loading
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.SetGameStart(true);
                    PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
                }
            }
            else
            {
                // Wait for everyone to load
                infoText.text = $"{PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CustomProperty.GAMESTARTTIME))
        {
            StartCoroutine(StartTimer());
        }
    }

    IEnumerator StartTimer()
    {
        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();
        while (PhotonNetwork.Time - loadTime < countDownTime)
        {
            int remainTime = (int)(countDownTime - (PhotonNetwork.Time - loadTime));
            infoText.text = (remainTime + 1).ToString();
            yield return null;
        }
        
        infoText.text = "Game Start!";
        GameStart();
        yield return new WaitForSeconds(1f);

        infoText.text = "";
    }
    
    public void GameStart()
    {
        Vector2 spawnPos = Random.insideUnitCircle * 30;
        PhotonNetwork.Instantiate("Player", new Vector3(spawnPos.x, 0,spawnPos.y), Quaternion.identity);
    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoaded())
            {
                loadCount++;
            }
        }
        return loadCount;
    }
}
