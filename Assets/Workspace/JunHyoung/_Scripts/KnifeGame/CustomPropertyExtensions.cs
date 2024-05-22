using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// CustomProperty Extension for Knife Game Mode
/// </summary>
public static class CustomPropertyExtensions
{
    public const string KILL = "Kill";
    public const string DEATH = "Death";

    public static int GetPlayerKillCount(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(KILL))
            return (int) properties[KILL];
        return 0;
    }

    public static void AddPlayerKillCount(this Player player)
    {
        PhotonHashtable properties = new PhotonHashtable { { KILL, GetPlayerKillCount(player)+1 } };
        player.SetCustomProperties(properties);
    }

    public static int GetPlayerDeathCount(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(DEATH))
            return (int) properties[DEATH];
        return 0;
    }

    public static void AddPlayerDeathCount(this Player player)
    {
        PhotonHashtable properties = new PhotonHashtable { { KILL, GetPlayerDeathCount(player) + 1 } };
        player.SetCustomProperties(properties);
    }
}