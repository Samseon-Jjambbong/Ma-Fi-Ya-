using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnifeGameResultBoard : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Button returnRoomButton;
    [SerializeField] Button returnLobbyButton;


    [Header("Settings")]
    [SerializeField] UserKillDeathEntry entryPrefab;
    [SerializeField] string TitleSceneName;
    [SerializeField] ScoreConfig scoreConfig;
    private void Awake()
    {
        returnRoomButton.onClick.AddListener(ReturnRoom);
        returnLobbyButton.onClick.AddListener(ReturnLobby);
    }

    void OnEnable()
    {
        InitResultBoard();
        CalculateScore();
        UpdateToDB();
    }

    private List<Player> sortedPlayers;
    void InitResultBoard()
    {
        var playerDic = KnifeGameManager.Instance.PlayerDic;

        sortedPlayers = playerDic.Values
           .OrderByDescending(player => player.GetPlayerKillCount())
           .ToList();

        foreach (var player in sortedPlayers)
        {
            UserKillDeathEntry entry = Instantiate(entryPrefab, content);
            entry.Set(player.NickName, player.GetPlayerKillCount(), player.GetPlayerDeathCount());
        }
    }

    // sortedPlayers 에서  PhotonNetwork.LocalPlayer.ActorNumber 의 순위에 맞게 점수 계산
    int score;
    void CalculateScore()
    {
        // LocalPlayer의 ActorNumber를 가져옴
        int localPlayerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // sortedPlayers에서 LocalPlayer의 순위를 찾음
        int rank = sortedPlayers.FindIndex(player => player.ActorNumber == localPlayerActorNumber) + 1;

        score = scoreConfig.GetScoreFromRank(rank);
        scoreText.text = $"+ {score}";
    }

    // DB에 점수 추가 
    void UpdateToDB()
    {
        if (FirebaseManager.UpdateRecord(score))
        {
            Debug.Log("UdateRecords Done!");
        }
        else
        {
            Debug.LogWarning("UdateRecords Faild!");
        }
    }


    void ReturnRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LoadLevel(TitleSceneName);
    }

    void ReturnLobby()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(TitleSceneName);
    }
}
