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
    public const string PLAYERROLE = "PlayerRole";
    public const string PLAYERCOLOR = "PlayerColor";
    public const string MAFIAREADY = "MafiaReady";
    public const string DEAD = "Dead";

    public static bool GetReady(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(READY))
            return (bool) properties[READY];
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
            return (bool) properties[LOAD];
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
            return (bool) properties[GAMESTART];
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
            return (double) properties[GAMESTARTTIME];
        return 0;
    }

    public static void SetGameStartTime(this Room room, double value)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { GAMESTARTTIME, value } };
        room.SetCustomProperties(propertiesToSet);
    }

    // Room Custom Property for Game Mode
    public static GameMode GetGameMode(this Room room)
    {
        PhotonHashtable properties = room.CustomProperties;
        if (properties.ContainsKey(GAMEMODE))
            return (GameMode) properties[GAMEMODE];
        return 0;
    }

    public static void SetGameMode(this RoomOptions room, GameMode value, bool setPropertyToLobby)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { GAMEMODE, value } };
        room.CustomRoomProperties = propertiesToSet;

        if (setPropertyToLobby)
            room.CustomRoomPropertiesForLobby = new string[] { GAMEMODE };
    }

    // Player Role
    public static MafiaRole GetPlayerRole(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(PLAYERROLE))
            return (MafiaRole) properties[PLAYERROLE];
        return 0;
    }

    public static void SetPlayerRole(this Player player, MafiaRole value)
    {
        PhotonHashtable properties = new PhotonHashtable { { PLAYERROLE, value } };
        player.SetCustomProperties(properties);
    }

    // Player Color
    public static Color GetPlayerColor(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(PLAYERCOLOR))
            return (Color) properties[PLAYERCOLOR];
        return Color.white;
    }

    public static void SetPlayerColor(this Player player, Color value)
    {
        PhotonHashtable properties = new PhotonHashtable { { PLAYERCOLOR, value } };
        player.SetCustomProperties(properties);
    }

    // Player Mafia Ready
    public static bool GetMafiaReady(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(MAFIAREADY))
            return (bool) properties[MAFIAREADY];
        return false;
    }

    public static void SetMafiaReady(this Player player, bool value)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { MAFIAREADY, value } };
        player.SetCustomProperties(propertiesToSet);
    }

    // Room Mafia Ready
    public static int GetMafiaReady(this Room room)
    {
        PhotonHashtable properties = room.CustomProperties;
        if (properties.ContainsKey(MAFIAREADY))
            return (int)properties[MAFIAREADY];
        return 0;
    }

    public static void IncrementMafiaReady(this Room room)
    {
        int value = room.GetMafiaReady() + 1;
        PhotonHashtable propertiesToSet = new PhotonHashtable { { MAFIAREADY, value } };
        room.SetCustomProperties(propertiesToSet);
    }

    public static void ResetMafiaReady(this Room room)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { MAFIAREADY, 0 } };
        room.SetCustomProperties(propertiesToSet);
    }

    // Player Dead (아마 마피아/칼전 둘다 사용 가능)
    public static bool GetDead(this Player player)
    {
        PhotonHashtable properties = player.CustomProperties;
        if (properties.ContainsKey(DEAD))
            return (bool) properties[DEAD];
        return false;
    }

    public static void SetDead(this Player player, bool value)
    {
        PhotonHashtable propertiesToSet = new PhotonHashtable { { DEAD, value } };
        player.SetCustomProperties(propertiesToSet);
    }
}
