using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChatTest :  MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    string channel;
    
    void OnEnable()
    {
        channel = PhotonNetwork.CurrentRoom.Name; //채팅 채널을 로비-룸 상태랑 게임중일 때 상태랑 구분지어야 하는지 확인해 볼것.
        chatClient.Subscribe(channel);
    }

    void Update()
    {
        chatClient.Service();
    }

    void IChatClientListener.DebugReturn( DebugLevel level, string message )
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnChatStateChange( ChatState state )
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnConnected()
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnGetMessages( string channelName, string [] senders, object [] messages )
    {
        //chatClient.Service(); 가 이거 호출함

        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnPrivateMessage( string sender, object message, string channelName )
    {
        //개인 메세지 수신
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnStatusUpdate( string user, int status, bool gotMessage, object message )
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnSubscribed( string [] channels, bool [] results )
    {
        //내가 채널 입장시
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnUnsubscribed( string [] channels )
    {
        //내가 채널에서 퇴장할 시
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnUserSubscribed( string channel, string user )
    {
        //유저가 채널 입장시
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnUserUnsubscribed( string channel, string user )
    {
        // 유저가 채널 퇴장시
        throw new System.NotImplementedException();
    }
}
