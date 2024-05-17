using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChatTest : MonoBehaviour, IChatClientListener
{
    [SerializeField] ChatEntry chatEntry;
    [SerializeField] Transform contents;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button buttonSend;

    ChatClient chatClient;
    string curChannelName;

    void Start()
    {
        buttonSend.onClick.AddListener(SendMessage);
        inputField.onSubmit.AddListener(SendMessage);
    }

    void OnEnable()
    {
       // ChatAppSettings.Instance.AppID;
        chatClient = new ChatClient(this);
        chatClient.AuthValues = new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        curChannelName = PhotonNetwork.CurrentRoom.Name; //채팅 채널을 로비-룸 상태랑 게임중일 때 상태랑 구분지어야 하는지 확인해 볼것.
    }

    void OnDisable()
    {
        if ( chatClient == null )
            return;
        chatClient.Disconnect();
    }

    void Update()
    {
        if ( chatClient == null )
            return;
        chatClient.Service();
    }

    private void SendMessage()
    {
        if ( string.IsNullOrEmpty(inputField.text) )
            return;

        chatClient.PublishMessage(curChannelName, inputField.text);
        inputField.text = "";
        inputField.ActivateInputField();
    }

    private new void SendMessage(string message )
    {
        if ( string.IsNullOrEmpty(message) )
            return;

        chatClient.PublishMessage(curChannelName, message);
        inputField.text = "";
        inputField.ActivateInputField();
    }

    void IChatClientListener.DebugReturn( DebugLevel level, string message )
    {
        Debug.Log(message);
    }

    void IChatClientListener.OnChatStateChange( ChatState state )
    {
        Debug.Log($"[ChatState : {state}]");
    }

    void IChatClientListener.OnConnected()
    {
        chatClient.Subscribe(curChannelName);
    }

    void IChatClientListener.OnDisconnected()
    {
        Debug.Log("Chat Disconnected");
    }

    void IChatClientListener.OnGetMessages( string channelName, string [] senders, object [] messages )
    {
        //chatClient.Service(); 가 이거 호출함
        if ( channelName.Equals(curChannelName) )
        {
            for(int i =0; i < senders.Length; i++ )
            {
                if ( senders [i] == null )
                    return;

                ChatEntry newChat = Instantiate(chatEntry, contents);
                //newChat.Set(senders [i], (string)messages [i]);
            }
        }
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
        ChatEntry newChat = Instantiate(chatEntry, contents);
       // newChat.Set(curChannelName, "입장하였습니다.");
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
