using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class MafiaPunManager : MonoBehaviourPunCallbacks
{
    [Header("Components")]
    [SerializeField] TMP_Text infoText;
    [SerializeField] MafiaRolesSO mafiaRolesSO;

    [Header("Values")]
    [SerializeField] int playerRadius;
    [SerializeField] int houseRadius;
    [SerializeField] List<Color> colorList;
    [SerializeField] float CountDownTime;
    [SerializeField] private int displayRoleTime;
    [SerializeField] private int roleUseTime;
    [SerializeField] private int voteTime;
    [SerializeField] private int skillTime;
    private bool everyoneReady;
    private bool masterReady;

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

    /******************************************************
    *                    Game Setup
    ******************************************************/
    #region Game Setup
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            infoText.text = $"{PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
            {
                if (PhotonNetwork.IsMasterClient)
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

    public override void OnRoomPropertiesUpdate(PhotonHashTable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CustomProperty.GAMESTARTTIME))
        {
            StartCoroutine(StartTime());
        }
    }

    IEnumerator StartTime()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnHouses(); // Spawn {PlayerCount} Houses
            //AssignRoles(PhotonNetwork.CurrentRoom.PlayerCount);
            AssignRoles(4); // TODO: CHANGE LATER
        }

        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();
        while (PhotonNetwork.Time - loadTime < CountDownTime)
        {
            int remainTime = (int) (CountDownTime - (PhotonNetwork.Time - loadTime));
            infoText.text = (remainTime + 1).ToString();
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
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoaded())
            {
                loadCount++;
            }
        }
        return loadCount;
    }

    private int PlayerMafiaReadyCount()
    {
        int count = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetMafiaReady())
            {
                count++;
            }
        }
        return count;
    }

    public void GameStart()
    {
        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(GameLoop());
        }
    }

    private void SpawnPlayer()
    {
        int angle = 0;
        if (Manager.Mafia.PlayerCount > 1)
        {
            angle = 180 / (Manager.Mafia.PlayerCount - 1);    // 각 플레이어의 간격의 각도
        }

        int playerNumber = -1;

        // 플레이어의 게임 입장 순번 찾아내기
        for (int i = 1; i <= playerDic.Count; i++)
        {
            if (playerDic.ContainsKey(i) && playerDic[i] == PhotonNetwork.LocalPlayer)
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

        if (playerNumber == playerDic.Count - 1)
        {
            currentAngle = 0;
        }

        // 순번에 맞는 플레이어의 위치 설정
        Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * playerRadius, 2.22f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * playerRadius);
        // PhotonNetwork.Instantiate를 통해 각 플레이어 캐릭터 생성, 센터를 바라보도록 rotation 설정
        GameObject player = PhotonNetwork.Instantiate("Mafia", pos, Quaternion.LookRotation(-pos));
        player.GetComponent<MafiaPlayer>().SetPlayerHouse(playerNumber);
        player.GetComponent<MafiaPlayer>().SetNickName(PhotonNetwork.PlayerList[playerNumber].NickName);
        Manager.Mafia.Player = player.GetComponent<MafiaPlayer>();
        Manager.Mafia.House = Manager.Mafia.Houses[playerNumber];
    }

    private void SpawnHouses()
    {
        int angle = 0;
        if (Manager.Mafia.PlayerCount > 1)
        {
            angle = 180 / (Manager.Mafia.PlayerCount - 1);    // 각 집의 간격의 각도
        }

        int currentAngle = 180;
        for (int i = 0; i < Manager.Mafia.PlayerCount; i++)
        {
            if (i == Manager.Mafia.PlayerCount - 1)
            {
                currentAngle = 0;
            }

            Vector3 pos = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad) * houseRadius, 1.8f, Mathf.Sin(currentAngle * Mathf.Deg2Rad) * houseRadius);
            GameObject houseGO = PhotonNetwork.InstantiateRoomObject("House", pos, Quaternion.LookRotation(pos));
            houseGO.GetComponent<House>().photonView.RPC("AddHouse", RpcTarget.All, i + 1);

            currentAngle -= angle;
        }
    }

    private void AssignRoles(int numPlayers)
    {
        // Get role pool
        MafiaRole[] roles = mafiaRolesSO.GetRoles(numPlayers);

        // Shuffle algorithm
        int n = roles.Length;
        for (int i = n - 1; i > 0; i--)
        {
            // Generate a random index j such that 0 <= j <= i
            int j = Random.Range(0, i + 1);

            // Swap array[i] with array[j]
            var temp = roles[i];
            roles[i] = roles[j];
            roles[j] = temp;
        }

        // Assign roles
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].SetPlayerRole(roles[i]);
            Manager.Mafia.Game.AddPlayer(roles[i]);
        }
    }
    #endregion

    /******************************************************
    *                    Game Loop
    ******************************************************/
    #region Game Loop
    private IEnumerator GameLoop()
    {
        // Delay
        yield return new WaitForSeconds(1);

        // Display role 
        photonView.RPC("DisplayRole", RpcTarget.All, displayRoleTime);
        //yield return new WaitUntil(() => Manager.Mafia.displayRoleFinished);
        yield return WaitForEveryone();

        // Loop
        while (true)
        {
            // Change to night
            Debug.Log("Night Phase Start");
            photonView.RPC("StartNightPhase", RpcTarget.All, skillTime);
            //yield return new WaitUntil(() => Manager.Mafia.nightPhaseFinished);
            yield return WaitForEveryone();
            Debug.Log("Night Phase End");

            yield return new WaitForSeconds(1);

            // Show Night Events
            Debug.Log("Night Events Start");
            photonView.RPC("ShowNightEvents", RpcTarget.MasterClient);
            //yield return new WaitUntil(() => Manager.Mafia.nightEventsFinished);
            yield return WaitForMaster();
            Debug.Log("Night Events End");

            yield return new WaitForSeconds(1);

            // Show Night Results
            Debug.Log("Night Results Start");
            photonView.RPC("ShowNightResults", RpcTarget.All);
            //yield return new WaitUntil(() => Manager.Mafia.nightResultsFinished);
            yield return WaitForEveryone();
            Debug.Log("Night Results End");

            yield return new WaitForSeconds(1);

            // Check Game Over
            yield return GameOverCheck();

            // Day Phase
            Debug.Log("Day Phase Start");
            photonView.RPC("StartDayPhase", RpcTarget.All, voteTime);
            //yield return new WaitUntil(() => Manager.Mafia.dayPhaseFinished);
            yield return WaitForEveryone();
            Debug.Log("Day Events End");
              
            yield return new WaitForSeconds(1);

            // Show Vote Result
            Debug.Log("Show Vote Results Start");
            photonView.RPC("CountVotes", RpcTarget.MasterClient);
            int voteResult = Manager.Mafia.sharedData.playerToKick;
            Debug.Log($"Vote Result : {voteResult}");
            if (voteResult != -1)
            {
                Debug.Log($"Player{voteResult} got kicked");
                photonView.RPC("ShowVoteResults", RpcTarget.All, voteResult);
                //yield return new WaitUntil(() => Manager.Mafia.voteResultsFinished);
                yield return WaitForEveryone();
            }
            else
            {
                Debug.Log("No one got kicked");
                yield return new WaitForSeconds(3);
            }
            
            Debug.Log("Show Vote Results End");

            yield return new WaitForSeconds(1);

            // Check Game Over
            yield return GameOverCheck();

            // Reset flags
            Manager.Mafia.ResetFlags();
        }
    }

    private IEnumerator GameOverCheck()
    {
        Debug.Log("Checking Game Over...");
        if (Manager.Mafia.GameResult != MafiaResult.None)
        {
            
            photonView.RPC("GameOver", RpcTarget.All, (int) Manager.Mafia.GameResult);
            Debug.Log($"Game Over: {Manager.Mafia.GameResult}");
            StopAllCoroutines();
        }
        else
        {
            Debug.Log("Game not over");
        }
        
        yield return null;
    }

    private IEnumerator WaitForEveryone()
    {
        foreach(var player in PhotonNetwork.PlayerList)
        {
            player.SetMafiaReady(false);
        }
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => PlayerMafiaReadyCount() == PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private IEnumerator WaitForMaster()
    {
        PhotonNetwork.MasterClient.SetMafiaReady(false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => PhotonNetwork.MasterClient.GetMafiaReady());
    }
    #endregion
}