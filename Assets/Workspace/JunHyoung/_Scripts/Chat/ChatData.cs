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
    public ChatData() { 
        name = string.Empty;
        message = string.Empty;
        nameColor = Color.white;
        messageColor = Color.white;
    }
    public ChatData(string name)
    {
        this.name = name;
        message = string.Empty;
        nameColor = Color.black;
        messageColor = Color.black;
    }

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

    /// <summary>
    /// for System Message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageColor"></param>
    public ChatData( string message, Color messageColor)
    {
        name = string.Empty;
        nameColor = Color.white;
        this.message = message;
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
        byte [] nameBytes = Encoding.UTF8.GetBytes(chatData.name);
        result.AddRange(BitConverter.GetBytes(nameBytes.Length));
        result.AddRange(nameBytes);

        // message 문자열 길이와 문자열 byte 배열을 추가
        byte [] messageBytes = Encoding.UTF8.GetBytes(chatData.message);
        result.AddRange(BitConverter.GetBytes(messageBytes.Length));
        result.AddRange(messageBytes);

        // nameColor 값을 byte 배열로 변환하여 추가
        result.AddRange(BitConverter.GetBytes(chatData.nameColor.r));
        result.AddRange(BitConverter.GetBytes(chatData.nameColor.g));
        result.AddRange(BitConverter.GetBytes(chatData.nameColor.b));
        result.AddRange(BitConverter.GetBytes(chatData.nameColor.a));

        // messageColor 값을 byte 배열로 변환하여 추가
        result.AddRange(BitConverter.GetBytes(chatData.messageColor.r));
        result.AddRange(BitConverter.GetBytes(chatData.messageColor.g));
        result.AddRange(BitConverter.GetBytes(chatData.messageColor.b));
        result.AddRange(BitConverter.GetBytes(chatData.messageColor.a));

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
        float r = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        float g = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        float b = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        float a = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        Color nameColor = new Color(r, g, b, a);

        // messageColor 읽기
        r = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        g = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        b = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        a = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        Color messageColor = new Color(r, g, b, a);

        return new ChatData(name, message, nameColor, messageColor);
    }
    #endregion
}
