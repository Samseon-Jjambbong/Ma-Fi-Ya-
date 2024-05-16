using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameChatPanel : ChatPanel
{
    [SerializeField] bool isChatable;
    public bool IsChatable { get { return isChatable; } set { isChatable = value; } }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //if LocalPlayer == mafia
        // 
    }

    

    private void OnEnter( )
    {
        if ( !isChatable )
            return;

        inputField.ActivateInputField();
    }

    protected override void SendMessage( string message )
    {
        if ( string.IsNullOrEmpty(message) )
            return;

        //chatClient.PublishMessage(curChannelName, message);
        ChatData newChat = new ChatData(PhotonNetwork.LocalPlayer.NickName, inputField.text);
        chatClient.PublishMessage(curChannelName, new ChatData(PhotonNetwork.LocalPlayer.NickName, inputField.text));
        inputField.text = "";
        inputField.ActivateInputField();
    }
}
