using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] RoomEntry roomEntryPrefab;

    private Dictionary<string, RoomEntry> roomDic;

    private void Awake()
    {
        roomDic = new Dictionary<string, RoomEntry>();
    }

    private void OnDisable()
    {
        for (int i = 0; i < roomContent.childCount; i++)
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }
        roomDic.Clear();
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (var roomInfo in roomList)
        {
            // 1 of 3 Scenarios:
            // 1. Room removed/hidden
            if (roomInfo.RemovedFromList || !roomInfo.IsOpen || !roomInfo.IsVisible)
            {
                if (!roomDic.ContainsKey(roomInfo.Name))
                    continue;
                
                RoomEntry roomEntry = roomDic[roomInfo.Name];
                roomDic.Remove(roomInfo.Name);
                Destroy(roomEntry.gameObject);
            }
            
            // 2. Room modified
            else if (roomDic.ContainsKey(roomInfo.Name))
            {
                roomDic[roomInfo.Name].UpdateInfo(roomInfo);
            }

            // 3. Room created
            else
            {
                RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomEntry.UpdateInfo(roomInfo);
                roomDic.Add(roomInfo.Name, roomEntry);
            }
        }
    }
}
