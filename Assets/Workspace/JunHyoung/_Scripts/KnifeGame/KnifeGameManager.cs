using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class KnifeGameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private static KnifeGameManager instance;
    public static KnifeGameManager Instance { get { return instance; } }

    [Header("Components")]
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI countDownText;
    // 게임 타이머 설정 : default 120초


    [Header("GameSettings")]
    [SerializeField] float countDownTime;
    [SerializeField] float gamePlayTime = 120f;
    // 플레이어 스폰 설정


    // 플레이어 리스폰 설정

    // 플레이어 스코어(킬,데스 관리) - Player Custom Properties?

    // 플레이어 스코어 랭킹

    // 떨어졌을 때 사망처리

    
    [SerializeField] GameObject playerprefab; //for Debugs
    [SerializeField] int playerRadius;

    [SerializeField] List<Color> colorList;
    private Dictionary<int, Player> playerDic;
    /******************************************************
    *                    Unity Events
    ******************************************************/
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.LocalPlayer.SetLoaded(true);
        playerDic = PhotonNetwork.CurrentRoom.Players;

        colorList = new List<Color> ();
        for (int i = 0; i < playerDic.Count; i++)
        {
            colorList.Add(new Color(Random.value, Random.value, Random.value, 1f));
        }
    }

    /******************************************************
    *                     PunCallbacks
    ******************************************************/

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)//로딩 완료
            {
                //게임 시작
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.SetGameStart(true);
                    PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time); //서버 시간을 기준으로 동기화
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false; // 방에 추가적인 참가를 막고 로비에서 보이지 않도록 함
                }
            }
            else
            {
                //다른 플레이어 로딩 기달리는 중
                infoText.text = $"Wait... {PlayerLoadCount()}/{PhotonNetwork.PlayerList.Length}";
            }
        }
    }
    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CustomProperty.GAMESTART))
        {
            StartCoroutine(GameStartRoutine());
        }
    }


    /******************************************************
    *                    Methods
    ******************************************************/

    private int PlayerLoadCount()
    {
        int loadcount = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {

            if (player.GetLoaded())
            {
                loadcount++;
            }
        }
        return loadcount;
    }

    IEnumerator GameStartRoutine()
    {
        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();
        while (PhotonNetwork.Time - loadTime < countDownTime)
        {
            int reamainTime = (int) (countDownTime - (PhotonNetwork.Time - loadTime));
            countDownText.text = (reamainTime + 1).ToString();
            yield return null;
        }

        countDownText.color = Color.green;
        countDownText.text = "Start!";
        //게임 시작시 필요한 로직들
        yield return new WaitForSeconds(1.0f);
        
        //infoText.text = "";


        //if (PhotonNetwork.IsMasterClient)
            //spawner.StartSpawnRoutine();

    }

    IEnumerator GameTimer(double loadTime)
    {
        while (PhotonNetwork.Time - loadTime < gamePlayTime)
        {
            int reamainTime = (int) (gamePlayTime - (PhotonNetwork.Time - loadTime));
            infoText.text = (reamainTime + 1).ToString();
            yield return null;
        }
    }


    private void SpawnPlayer()
    {
        int angle = 360 / (Manager.Mafia.PlayerCount - 1);    // 각 플레이어의 간격의 각도

        int playerNumber = -1;

        // 플레이어의 게임 입장 순번 찾아내기
        for (int i = 1; i <= playerDic.Count; i++)
        {
            if (playerDic[i] == PhotonNetwork.LocalPlayer)
            {
                playerNumber = i - 1;
            }
        }

        if (playerNumber == -1)
        {
            Debug.Log("Can't found LocalPlayer Number");
            return;
        }

        int currentAngle = 360 - angle * playerNumber;

        if (playerNumber == playerDic.Count - 1)
        {
            currentAngle = 0;
        }

        // 순번에 맞는 플레이어의 위치 설정
        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * playerRadius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * playerRadius);

        // PhotonNetwork.Instantiate를 통해 각 플레이어 캐릭터 생성, 센터를 바라보도록 rotation 설정
        GameObject player = Instantiate(playerprefab, pos, Quaternion.LookRotation(-pos));
        //색깔 설정 - 은 플레이어에서 
    }

    /******************************************************
    *                    IPunObservable
    ******************************************************/
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
