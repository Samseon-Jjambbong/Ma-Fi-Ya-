using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.InputSystem.XR.Haptics;
using ExitGames.Client.Photon;

public class KillLogData
{

    /******************************************************
    *                    Data Field
    ******************************************************/
    public byte type; // 0:death, 1:kill
    public string sender;
    public string target;

    /******************************************************
    *                    Constructors
    ******************************************************/
    /// <summary>
    /// KillLogData 생성 메서드
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="target"></param>
    /// <param name="type"> 0:death, 1:kill</param>
    public KillLogData(string sender, string target = null, byte type = 0)
    {
        this.type = type;
        this.sender = sender;
        this.target = target;
    }

    /******************************************************
    *                Serialize / Deserialize
    ******************************************************/
    #region Serilaze/Deserialize
    public static byte[] Serialize(object obj)
    {
        return Serialize((KillLogData) obj);
    }
    private static byte[] Serialize(KillLogData killlog)
    {
        // List 초기화 밑 type 데이터 추가
        List<byte> result = new List<byte>
        {
            killlog.type
        };

        // sender 문자열 길이와 문자열 byte 배열을 추가
        byte[] nameBytes = Encoding.UTF8.GetBytes(killlog.sender);
        result.AddRange(BitConverter.GetBytes(nameBytes.Length));
        result.AddRange(nameBytes);

        // target 문자열 길이와 문자열 byte 배열을 추가
        byte[] messageBytes = Encoding.UTF8.GetBytes(killlog.target);
        result.AddRange(BitConverter.GetBytes(messageBytes.Length));
        result.AddRange(messageBytes);


        return result.ToArray();
    }

    public static KillLogData Deserialize(byte[] data)
    {
        int offset = 0;

        //byte type  읽기
        byte type = data[offset];
        offset += sizeof(byte);

        // sender 문자열 길이 읽기
        int senderLength = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);

        // sender 문자열 읽기
        string sender = Encoding.UTF8.GetString(data, offset, senderLength);
        offset += senderLength;

        // target 문자열 길이 읽기
        int targetLength = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);

        // target 문자열 읽기
        string target = Encoding.UTF8.GetString(data, offset, targetLength);
        offset += targetLength;


        return new KillLogData(sender, target, type);
    }
    #endregion
}
