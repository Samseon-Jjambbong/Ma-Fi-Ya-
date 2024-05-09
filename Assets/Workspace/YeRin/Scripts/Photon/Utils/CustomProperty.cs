using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    public const string READY = "Ready";
    public const string LOAD = "Load";

    public static bool GetReady(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        if (customProperty.TryGetValue(READY, out object value))
        {
            return (bool)value;
        }
        else
        {
            return false;
        }
    }

    public static void SetReady(this Player player, bool value) 
    {
        PhotonHashTable customProperty = new PhotonHashTable();
        customProperty[READY] = value;    // 이미 있을 경우 충돌이 날 수 있으므로 예방을 위해 Add 사용 대신 이렇게.
        player.SetCustomProperties(customProperty);
    }

    public static bool GetLoad(this Player player) 
    {
        PhotonHashTable customProperty = player.CustomProperties;
        if (customProperty.TryGetValue(LOAD, out object value))
        {
            return (bool)value;
        }
        else
        {
            return false;
        }
    }

    public static void SetLoad(this Player player, bool value)
    {
        PhotonHashTable customProperty = new PhotonHashTable();
        customProperty[LOAD] = value;    // 이미 있을 경우 충돌이 날 수 있으므로 예방을 위해 Add 사용 대신 이렇게.
        player.SetCustomProperties(customProperty);
    }

    public const string GAMESTART = "GameStart";

    public static bool GetGameStart(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(GAMESTART, out object value))
        {
            return (bool)value;
        }
        else
        {
            return false;
        }
    }

    public static void SetGameStart(this Room room, bool value)
    {
        PhotonHashTable customProperty = new PhotonHashTable();
        customProperty[GAMESTART] = value;
        room.SetCustomProperties(customProperty);
    }

    public const string GAMESTARTTIME = "GameStartTime";

    public static double GetGameStartTime(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(GAMESTARTTIME, out object value))
        {
            return (double)value;
        }
        else
        {
            return 0;
        }
    }

    public static void SetGameStartTime(this Room room, double value)
    {
        PhotonHashTable customProperty = new PhotonHashTable();
        customProperty[GAMESTARTTIME] = value;
        room.SetCustomProperties(customProperty);
    }
}
