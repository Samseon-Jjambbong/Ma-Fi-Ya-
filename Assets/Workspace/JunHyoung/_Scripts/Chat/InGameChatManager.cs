using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] bool isChatable;
    public bool IsChatable { get { return isChatable; } set { isChatable = value; inputField.interactable = isChatable; } }

    [SerializeField] ChatEntry chatEntry;
    [SerializeField] Transform contents;
    [SerializeField] RectTransform rectTransform;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button buttonFixSize;

    private ChatClient chatClient;
    private string curChannelName;
    private string mafiaChannelName;

    [SerializeField] public bool isMafia;
    [SerializeField] private Color nickNameColor;
    [SerializeField] Color mafiaMessageColor;
    [SerializeField] Color systemMessageColor;

    [Header("For Debug")]
    [SerializeField] public bool isDay;

    /******************************************************
    *                    Unity Events
    ******************************************************/
    #region Unity Events
    void Start()
    {
        PhotonPeer.RegisterType(typeof(ChatData), 100, ChatData.Serialize, ChatData.Deserialize);

        buttonFixSize.onClick.AddListener(FixSize);
        inputField.onSubmit.AddListener(SendMessage);
    }

    void OnEnable()
    {
        InitChatClient();
    }

    void OnDisable()
    {
        if ( chatClient == null )
            return;

        foreach ( Transform child in contents )
        {
            Destroy(child.gameObject);
        }

        chatClient.Disconnect();
    }

    void Update()
    {
        if ( chatClient == null )
            return;
        chatClient.Service();

        if ( !isChatable )
            return;

        if ( Input.GetKeyDown(KeyCode.Return) ) //the key used to accept a line of text. osMac calls this Return. A PC calls this Enter. There is no KeyCode.Enter(...)
        {
            inputField.ActivateInputField();
        }
    }

    #endregion

    /******************************************************
    *                    Methods
    ******************************************************/

    #region Methods

    void InitChatClient()
    {
        chatClient = new ChatClient(this);
        chatClient.AuthValues = new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);

        ChatEntry newChat = Instantiate(chatEntry, contents);
        newChat.SetChat(new ChatData(" ", $"Connect On Chatting Channel...", Color.black, Color.yellow));

        chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());

        curChannelName = PhotonNetwork.CurrentRoom.Name + "InGame";
        mafiaChannelName = curChannelName + "Night";

        //isMafia = (PhotonNetwork.LocalPlayer.GetRole() == "Mafia";
        //nickNameColor = PhotonNetwork.LocalPlayer.GetColor();
    }

    private void FixSize() // 버튼 클릭 시
    {
        IsChatable = !IsChatable;

        //rectTransform.sizeDelta =new Vector2();

        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
    }

    new void SendMessage( string message ) // InputField 에서 Enter 시
    {
        if ( !isChatable )
            return;

        if ( string.IsNullOrEmpty(message) )
            return;

        if ( !isDay && isMafia )
        //if(!MafiaManager.Instance.IsDay && isMafia ) // 낮이 아니고, 마피아라면 마피아 채널에
        {
            chatClient.PublishMessage(mafiaChannelName, new ChatData(PhotonNetwork.LocalPlayer.NickName, inputField.text, nickNameColor, mafiaMessageColor));
        }
        else // 이외라면 일반 채팅 채널에
        {
            chatClient.PublishMessage(curChannelName, new ChatData(PhotonNetwork.LocalPlayer.NickName, inputField.text, nickNameColor));
        }

        inputField.text = "";
        inputField.ActivateInputField();
        chatClient.PublicChannels.Clear();
    }

    public void PublishMessage( ChatData chatdata )
    {
        if ( !PhotonNetwork.IsMasterClient ) return;

        chatClient.PublishMessage(curChannelName, chatdata);
    }


    #endregion
    /******************************************************
    *              IChatClientListener Callbacks
    ******************************************************/

    #region Callbacks
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
        // PublishSubscribers = true 가 아니면 OnuserSubscribe 가 콜백되지 않음
        chatClient.Subscribe(curChannelName, 0, -1,
            new ChannelCreationOptions() { PublishSubscribers = true, MaxSubscribers = PhotonNetwork.CurrentRoom.MaxPlayers });
        if ( isMafia )
        {
            chatClient.Subscribe(mafiaChannelName);
        }
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
            for ( int i = 0; i < senders.Length; i++ )
            {
                if ( senders [i] == null )
                    return;
                ChatData chatData = ( ChatData ) messages [i];
                ChatEntry newChat = Instantiate(chatEntry, contents);
                newChat.SetChat(chatData);
            }
        }

        // 마피아일 때만 마피아 채널의 메세지 수신함
        if ( !isMafia )
            return;

        if ( channelName.Equals(mafiaChannelName) )
        {
            for ( int i = 0; i < senders.Length; i++ )
            {
                if ( senders [i] == null )
                    return;
                ChatEntry newChat = Instantiate(chatEntry, contents);
                ChatData chatData = ( ChatData ) messages [i];
                newChat.SetChat(chatData);
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
        newChat.SetChat(new ChatData(" ", $"You Entered Room:{curChannelName}", Color.black, Color.green));
    }

    void IChatClientListener.OnUnsubscribed( string [] channels )
    {
        //내가 채널에서 퇴장할 시
    }

    void IChatClientListener.OnUserSubscribed( string channel, string user )
    {
        ChatEntry newChat = Instantiate(chatEntry, contents);
        //유저가 채널에 입장 할 시
        newChat.SetChat(new ChatData(" ", $"{user} was Entered Room", Color.black, Color.blue));
    }

    void IChatClientListener.OnUserUnsubscribed( string channel, string user )
    {
        // 유저가 채널 퇴장시
        ChatEntry newChat = Instantiate(chatEntry, contents);
        newChat.SetChat(new ChatData(" ", $"{user} was Exit Room", Color.black, Color.red));
    }
    #endregion
}
