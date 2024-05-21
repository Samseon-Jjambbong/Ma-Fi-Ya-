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

    [Header("UI")]
    [SerializeField] GameObject killScoreBoardUI;
    [SerializeField] GameObject gameResultUI;


    [Header("GameSettings")]
    [SerializeField] float countDownTime;
    [SerializeField] float gamePlayTime = 120f;


    // 플레이어 리스폰 설정



    // 플레이어 스코어(킬,데스 관리) - Player Custom Properties?
    


    // 플레이어 스코어 랭킹

    // 떨어졌을 때 사망처리

    
    [SerializeField] GameObject playerprefab; //for Debugs
    [SerializeField] int playerRadius;

    [SerializeField] List<Color> colorList;
    [SerializeField] Dictionary<int, Player> playerDic;
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

    public void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoaded(true);
        playerDic = new Dictionary<int, Player>();
        playerDic = PhotonNetwork.CurrentRoom.Players;
        Debug.Log(PhotonNetwork.CurrentRoom.Players);

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
                    Debug.Log("Room Set Game Start");
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
            Debug.Log("Start GameStartRoutine");
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

    public void StartGameRoutine()
    {
        Debug.Log("Debugging StartRoutine...");
        PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
        StartCoroutine(GameStartRoutine());
    }

    IEnumerator GameStartRoutine()
    {
        yield return new WaitForSeconds(0.1f);//wait for Properties Set   
        SpawnPlayer();
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
        countDownText.text = "";
        StartCoroutine (GameTimer());
    }

    IEnumerator GameTimer()
    {
        PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
        yield return new WaitForSeconds(0.1f);
        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();

        //게임 플레이 카운트 다운
        while (PhotonNetwork.Time - loadTime < gamePlayTime)
        {
            int reamainTime = (int) (gamePlayTime - (PhotonNetwork.Time - loadTime));
            infoText.text = reamainTime.ToString();
            yield return null;
        }

        //게임 종료 루틴
        countDownText.color = Color.red;
        countDownText.text = "Finish!!";

        //플레이어 조작 OFF

        if (PhotonNetwork.IsMasterClient)
        {
            //킬스코어 기반으로 점수 계산

           // int score = 0;
            // Firebase DB에 점수 추가
           // FirebaseManager.UpdateRecord(score);

        }

        // 게임 종료 UI POPUP
    }

    // 플레이어 초기 스폰 설정
    private void SpawnPlayer()
    {
        // 플레이어 각도 계산

        #region 플레이어 각도 계산
        int angle = 360 / (PhotonNetwork.CurrentRoom.PlayerCount);    // 각 플레이어의 간격의 각도

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

        int currentAngle;
        if (playerNumber == playerDic.Count-1)
        {
            currentAngle = 0; // 마지막 플레이어일 때는 0도에 배치
        }
        else
        {
            currentAngle = 360 - (angle * (playerNumber + 1)); // 다른 플레이어들은 360도에서 차례대로 각도 빼기
        }
        #endregion
        Debug.Log(currentAngle);
        // 순번에 맞는 플레이어의 위치 설정
        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * playerRadius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * playerRadius);

        // PhotonNetwork.Instantiate를 통해 각 플레이어 캐릭터 생성, 센터를 바라보도록 rotation 설정
        // GameObject player = Instantiate(playerprefab, pos, Quaternion.LookRotation(-pos));
        GameObject player = PhotonNetwork.Instantiate("Mafia", pos, Quaternion.LookRotation(-pos));

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
