using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    public const string READY = "Ready";
    public const string LOAD = "Load";
    public const string GAMESTART = "GameStart";
    public const string GAMESTARTTIME = "GameStartTime";
    public const string GAMEMODE = "GameMode";
    
    public static bool GetReady(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(READY))
            return (bool)properties[READY];
        return false;
    }

    public static void SetReady(this Player player, bool value)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { READY, value } };
        player.SetCustomProperties(propertiesToSet);
    }

    public static bool GetLoaded(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(LOAD))
        {
            Debug.Log((bool)properties[LOAD]);
            return (bool)properties[LOAD];
        }
            
        return false;
    }

    public static void SetLoaded(this Player player, bool value)
    {
        PhotonHashtable properties = new PhotonHashtable { { LOAD, value } };
        player.SetCustomProperties(properties);
    }

    public static bool GetGameStart(this Room room)
    {
        PhotonHashtable properties = room.CustomProperties;
        if (properties.ContainsKey(GAMESTART))
            return (bool)properties[GAMESTART];
        return false;
    }
    
    public static void SetGameStart(this Room room, bool value)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { GAMESTART, value } };
        room.SetCustomProperties(propertiesToSet);
    }

    public static double GetGameStartTime(this Room room)
    {
        PhotonHashtable properties = room.CustomProperties;
        if (properties.ContainsKey(GAMESTARTTIME))
            return (double)properties[GAMESTARTTIME];
        return 0;
    }
    
    public static void SetGameStartTime(this Room room, double value)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { GAMESTARTTIME, value } };
        room.SetCustomProperties(propertiesToSet);
    }
    
    // Room Custom Property for Game Mode
    public static GameMode GetGameMode( this Room room )
    {
        PhotonHashtable properties = room.CustomProperties;
        if (properties.ContainsKey(GAMEMODE))
            return (GameMode)properties[GAMEMODE];
        return 0;
    }
    
    public static void SetGameMode(this RoomOptions room, GameMode value, bool setPropertyToLobby)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { GAMEMODE, value } };
        room.CustomRoomProperties = propertiesToSet;
        
        if ( setPropertyToLobby )
            room.CustomRoomPropertiesForLobby = new string [] { GAMEMODE };
    }
}
