using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


// Custom Type을 PublichMessage 할려면, byte aray || stream buffer로 직렬화&역직렬화 하는 메서드를 작성한 후
// PhotonPeer.RegisterType에 등록해야함...

public class ChatData 
{
   /******************************************************
   *                    Data Field
   ******************************************************/

    public string name;
    public string message;

    public Color nameColor;
    public Color messageColor;


    /******************************************************
    *                    Constructors
    ******************************************************/
    #region Constructors
    public ChatData( string name, string message )
    {
        this.name = name;
        this.message = message;
        this.nameColor = Color.black;
        this.messageColor = Color.black;
    }

    public ChatData( string name, string message, Color nameColor )
    {
        this.name = name;
        this.message = message;
        this.nameColor = nameColor;
        this.messageColor = Color.black;
    }

    public ChatData( string name, string message, Color nameColor, Color messageColor )
    {
        this.name = name;
        this.message = message;
        this.nameColor = nameColor;
        this.messageColor = messageColor;
    }
    #endregion

    /******************************************************
    *                Serialize / Deserialize
    ******************************************************/
    #region Serilaze/Deserialize
    public static byte [] Serialize( object obj )
    {
        return Serialize(( ChatData ) obj);
    }
    private static byte [] Serialize( ChatData chatData )
    {
        List<byte> result = new List<byte>();

        // name 문자열 길이와 문자열 byte 배열을 추가
        result.AddRange(BitConverter.GetBytes(chatData.name.Length));
        result.AddRange(Encoding.UTF8.GetBytes(chatData.name));

        // message 문자열 길이와 문자열 byte 배열을 추가
        result.AddRange(BitConverter.GetBytes(chatData.message.Length));
        result.AddRange(Encoding.UTF8.GetBytes(chatData.message));

        // nameColor 값을 byte 배열로 변환하여 추가
        result.Add(( byte ) chatData.nameColor.r);
        result.Add(( byte ) chatData.nameColor.g);
        result.Add(( byte ) chatData.nameColor.b);
        result.Add(( byte ) chatData.nameColor.a);

        // messageColor 값을 byte 배열로 변환하여 추가
        result.Add(( byte ) chatData.messageColor.r);
        result.Add(( byte ) chatData.messageColor.g);
        result.Add(( byte ) chatData.messageColor.b);
        result.Add(( byte ) chatData.messageColor.a);

        return result.ToArray();
    }

    public static ChatData Deserialize( byte [] data )
    {
        int offset = 0;

        // name 문자열 길이 읽기
        int nameLength = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);

        // name 문자열 읽기
        string name = Encoding.UTF8.GetString(data, offset, nameLength);
        offset += nameLength;

        // message 문자열 길이 읽기
        int messageLength = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);

        // message 문자열 읽기
        string message = Encoding.UTF8.GetString(data, offset, messageLength);
        offset += messageLength;

        // nameColor 읽기
        byte r = data [offset++];
        byte g = data [offset++];
        byte b = data [offset++];
        byte a = data [offset++];
        Color nameColor = new Color(r, g, b, a);

        // messageColor 읽기
        r = data [offset++];
        g = data [offset++];
        b = data [offset++];
        a = data [offset++];
        Color messageColor = new Color(r, g, b, a);

        return new ChatData(name, message, nameColor, messageColor);
    }
    #endregion
}
