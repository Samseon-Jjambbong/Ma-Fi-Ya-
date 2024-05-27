using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public class KnifeGameScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject panel;
    [SerializeField] RectTransform content;
    [SerializeField] UserKillDeathEntry entryPrefab;

    [SerializeField] bool isSorting;

    Dictionary<int, Player> playerDic;
    Dictionary<int, UserKillDeathEntry> entryDic;
    public Dictionary<int,UserKillDeathEntry> EntryDic {  get { return entryDic; } }
    UserKillDeathEntry changedEntry;

    bool isInit = false;

    public void ActivePanel()
    {
        if (!isInit)
            InitScoreBoard();

        panel.SetActive(!panel.gameObject.activeSelf);
    }

    void InitScoreBoard()
    {
        isInit = true;
        entryDic = new Dictionary<int, UserKillDeathEntry>();
        playerDic = KnifeGameManager.Instance.PlayerDic;

        foreach (var player in playerDic.Values)
        {
            UserKillDeathEntry entry = Instantiate(entryPrefab, content);
            entry.Set(player.NickName, player.GetPlayerKillCount(), player.GetPlayerDeathCount());
            entryDic.Add(player.ActorNumber, entry);
        }
    }

    public void SortScoreBoard()
    {
        if (!isInit)
        {
            Debug.LogWarning("Scoreboard not initialized.");
            return;
        }

        List<UserKillDeathEntry> sortedEntries = entryDic.Values
            .OrderByDescending(entry => entry.GetKillCount())
            .ToList();

        // 정렬된 리스트를 기반으로 UI의 순서를 다시 설정
        for (int i = 0; i < sortedEntries.Count; i++)
        {
            sortedEntries[i].transform.SetSiblingIndex(i);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (!isInit)
            return;
        
        if (changedProps.ContainsKey(CustomPropertyExtensions.KILL) || changedProps.ContainsKey(CustomPropertyExtensions.DEATH))
        {
            changedEntry = entryDic[targetPlayer.ActorNumber];
            changedEntry.UpdateEntry(targetPlayer.GetPlayerKillCount(), targetPlayer.GetPlayerDeathCount());
            if(isSorting)
            {
                SortScoreBoard();
            }
        }
    }

}
