using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public class KnifeGameScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] RectTransform content;
    [SerializeField] UserKillDeathEntry entryPrefab;

    // KnifeGameManager.Instance.PlayerDic;
    Dictionary<int, Player> playerDic;
    Dictionary<int, UserKillDeathEntry> entryDic;
    UserKillDeathEntry changedEntry;

    public override void OnEnable()
    {
        base.OnEnable();
     
        InitScoreBoard();
    }

    void InitScoreBoard()
    {
        playerDic = KnifeGameManager.Instance.PlayerDic;

        foreach (var player in playerDic.Values)
        {
            UserKillDeathEntry entry = Instantiate(entryPrefab, content);
            entry.Set(player.NickName, player.GetPlayerKillCount(), player.GetPlayerDeathCount());
            entryDic.Add(player.ActorNumber, entry);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomPropertyExtensions.KILL) || changedProps.ContainsKey(CustomPropertyExtensions.DEATH))
        {
            changedEntry = entryDic[targetPlayer.ActorNumber];
            changedEntry.UpdateEntry(targetPlayer.GetPlayerKillCount(), targetPlayer.GetPlayerDeathCount());
        }
    }
}
