using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using Photon.Pun.UtilityScripts;
using UnityEngine.InputSystem;
using System;
using Random = UnityEngine.Random;

public class MafiaPunManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float CountDownTime;

    [SerializeField] int playerRadius;
    [SerializeField] int houseRadius;
    [SerializeField] List<Color> colorList;

    private Dictionary<int, Player> playerDic;

    [Header("Game Flow")]
    [SerializeField] private int displayRoleTime;
    [SerializeField] private int roleUseTime;
    [SerializeField] private int voteTime;
    [SerializeField] private int skillTime;

    [Header("Game Logic")]
    [SerializeField] MafiaRolesSO mafiaRolesSO;

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
            //RandomizeRoles(PhotonNetwork.CurrentRoom.PlayerCount);
            RandomizeRoles(4); // TODO: CHANGE LATER
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
        AssignRole();
        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient ) 
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator GameLoop()
    {
        // Delay
        yield return new WaitForSeconds(1);

        // Display role
        photonView.RPC("DisplayRole", RpcTarget.All, displayRoleTime);
        yield return new WaitForSeconds(displayRoleTime);

        while (true)
        {
            // Change to night
            photonView.RPC("StartNightPhase", RpcTarget.All, skillTime);

            yield return new WaitForSeconds(skillTime + 1);

            // Show Night Events
            Debug.Log("Night Events Start");
            photonView.RPC("ShowNightEvents", RpcTarget.All);

            // Wait until everyone finishes showing events
            while (PlayerMafiaReadyCount() != PhotonNetwork.PlayerList.Length)
            {
                yield return null;
            }
            Debug.Log("Night Events End");

            yield return new WaitForSeconds(1);

            // Day Phase
            photonView.RPC("StartDayPhase", RpcTarget.All, voteTime);
            yield return new WaitForSeconds(voteTime + 1);

            // Show Vote Result
            photonView.RPC("ShowVoteResults", RpcTarget.All);
        }
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
        player.GetComponent<MafiaPlayer>().SetNickName(PhotonNetwork.PlayerList [playerNumber].NickName);
        Manager.Mafia.Player = player.GetComponent<MafiaPlayer>();
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
            //houseGO.GetComponent<House>().houseOwnerId = i + 1;
            houseGO.GetComponent<House>().photonView.RPC("AddHouse", RpcTarget.All, i + 1);

            currentAngle -= angle;
        }
    }

    private void RandomizeRoles(int numPlayers)
    {
        // Get role pool
        MafiaRole[] roles = mafiaRolesSO.GetRoles(numPlayers);
        
        // Shuffle list algorithm
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

        int[] arr = new int[roles.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = (int) roles[i];
        }

        // Update role list on master
        PhotonNetwork.CurrentRoom.SetMafiaRoleList(arr);
    }

    private void AssignRole()
    {
        int[] roles = PhotonNetwork.CurrentRoom.GetMafiaRoleList();
        MafiaRole role = (MafiaRole)roles[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        PhotonNetwork.LocalPlayer.SetPlayerRole(role);
        Manager.Mafia.Game.AddPlayer(role);
    }
}

