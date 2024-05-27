using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public enum KnifeLength { Short, Middle, Long }

public class KnifeGameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private static KnifeGameManager instance;
    public static KnifeGameManager Instance { get { return instance; } }


    [Header("Components")]
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI countDownText;
    // 게임 타이머 설정 : default 120초

    [Header("UI")]
    [SerializeField] KnifeGameScoreBoard scoreBoardUI;
    [SerializeField] KnifeGameResultBoard gameResultUI;
    [SerializeField] WeaponUI weaponUI;
    public WeaponUI WeaponUI => weaponUI;


    [Header("GameSettings")]
    [SerializeField] float countDownTime;
    [SerializeField] float gamePlayTime = 120f;

    [Header("Sounds")]
    [SerializeField] AudioClip gameReadyBGM;
    [SerializeField] AudioClip InGameBGM;
    [SerializeField] AudioClip gameStartSFX;
    [SerializeField] AudioClip gameFinishSFX;

    // Knife Player 관련 정보
    public KnifePlayer curPlayerController;
 
    KnifeLength knife;
    public KnifeLength Knife { get { return knife; } set { knife = value; } }
    // 플레이어 리스폰 설정

    // 플레이어 스코어 랭킹

    // 떨어졌을 때 사망처리

    [SerializeField] int playerRadius; // 스폰 거리 간격

    [SerializeField] List<Color> colorList;
    private Dictionary<int, Player> playerDic;
    public Dictionary<int, Player> PlayerDic { get { return playerDic; } }
    /******************************************************
    *                    Unity Events
    ******************************************************/
    #region Unity Events

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
        Manager.Sound.PlayBGM(gameReadyBGM);
    }

    //for turn on/off ScoreBoardUI
    /// <summary>
    /// 게임 흐름에 따라 ScoreBoard 를 띄워줘도 될지 안될지 설정. 
    /// </summary>
    bool isSBInteractable; 
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && isSBInteractable)
        {
            scoreBoardUI.ActivePanel();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable(); 

        // 커스텀 프로퍼티 초기화
        PhotonNetwork.LocalPlayer.ClearKillDeathCount();
    }
    
    #endregion
    /******************************************************
    *                     PunCallbacks
    ******************************************************/

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)//로딩 완료
            {
                infoText.text = $"Wait...";
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

    //for Debug
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
        Manager.Sound.PlaySFX(gameStartSFX);
        Manager.Sound.PlayBGM(InGameBGM);
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
        yield return new WaitForSeconds(0.2f);
        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();

        //게임 플레이 카운트 다운 시작
        curPlayerController.CanMove = true;
        isSBInteractable = true;
        while (PhotonNetwork.Time - loadTime < gamePlayTime)
        {
            int reamainTime = (int) (gamePlayTime - (PhotonNetwork.Time - loadTime));
            infoText.text = reamainTime.ToString();
            yield return null;
        }

        //게임 종료 루틴
        GameFinish();
    }

    void GameFinish()
    {
        isSBInteractable = false;
        countDownText.color = Color.red;
        countDownText.text = "Finish!!";
        Manager.Sound.PlaySFX(gameFinishSFX);

        //플레이어 조작 OFF
        curPlayerController.CanMove = false;

        // 게임 종료 UI POPUP
        gameResultUI.gameObject.SetActive(true);
    }

    // 플레이어 초기 스폰 설정
    private void SpawnPlayer()
    {
        // 플레이어 각도 계산

        #region 플레이어 각도 계산
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int angle = 360 / playerCount;    // 각 플레이어의 간격의 각도

        int playerNumber = playerDic.FirstOrDefault(kv => kv.Value == PhotonNetwork.LocalPlayer).Key - 1;

        if (playerNumber < 0)
        {
            Debug.Log("Can't found LocalPlayer Number");
            return;
        }

        int currentAngle = (playerNumber == playerCount - 1) ? 0 : 360 - (angle * (playerNumber + 1));
        float radianAngle = currentAngle * Mathf.Deg2Rad;
        #endregion


        // 순번에 맞는 플레이어의 위치 설정
        Vector3 pos = new Vector3(Mathf.Cos(radianAngle) * playerRadius, 2.22f, Mathf.Sin(radianAngle) * playerRadius);
        GameObject player = PhotonNetwork.Instantiate("Knife", pos, Quaternion.LookRotation(-pos)); //플레이어
        curPlayerController = player.GetComponent<KnifePlayer>();

        curPlayerController.SetNickName(PhotonNetwork.PlayerList[playerNumber].NickName);
        curPlayerController.photonView.RPC("SetWeapon", RpcTarget.MasterClient, KnifeLength.Short);
        weaponUI.gameObject.SetActive(true);
    }

    /******************************************************
    *                    IPunObservable
    ******************************************************/
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
