using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class KnifeGameChatManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    static KnifeGameChatManager instance;
    static public KnifeGameChatManager Instance { get { return instance; } }

    private ChatClient chatClient;
    private string userName;
    private string curChannelName;

    [Header("Chat")]
    [SerializeField] ChatEntry chatEntry;
    [SerializeField] Transform contents; // chatEntry 가 생성될 부모
    [SerializeField] RectTransform rectTransform; // 채팅창 사이즈 조절용

    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button buttonFixSize;

    [Header("Message Color Settings")]
    [SerializeField] Color nickNameColor;
    [SerializeField] Color systemMessageColor;

    /******************************************************
    *                    Unity Events
    ******************************************************/
    #region Unity Events

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PhotonPeer.RegisterType(typeof(ChatData), (byte)'C', ChatData.Serialize, ChatData.Deserialize);

        buttonFixSize.onClick.AddListener(FixSize);
        inputField.onSubmit.AddListener(SendMessage);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        InitChatClient();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (chatClient == null)
            return;

        chatClient.Disconnect();
    }

    void Update()
    {
        if (chatClient == null)
            return;

        chatClient.Service();

        if (Input.GetKeyDown(KeyCode.Return)) //the key used to accept a line of text. osMac calls this Return. A PC calls this Enter. There is no KeyCode.Enter(...)
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
        userName = PhotonNetwork.LocalPlayer.NickName;
        chatClient = new ChatClient(this);
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(userName);

        ChatEntry newChat = Instantiate(chatEntry, contents);
        newChat.SetChat(new ChatData(" ", $"Connect On Chatting Channel...", Color.black, Color.yellow));

        chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());

        curChannelName = $"{PhotonNetwork.CurrentRoom.Name}InGame"; // 근데 이렇게 채널명을 정하는 방식에는 문제가 있음
        // 누가 똑같은 이름으로 방 만들면 로비의 방 화면에서 해당 게임의 메세지들을 수신 가능함
    } 

    enum SizeState { Default, MAX, MIN }
    [Header("Chat Size Setting")]
    [SerializeField] SizeState sizeState = SizeState.Default;
    [SerializeField] Vector2 deafaultSize;
    [SerializeField] Vector2 maxSize;
    [SerializeField] Vector2 minSize;
    private void FixSize() // 버튼 클릭 시
    {
        switch (sizeState)
        {
            case SizeState.Default:
                sizeState = SizeState.MAX;
                rectTransform.sizeDelta = maxSize;
                break;
            case SizeState.MAX:
                sizeState = SizeState.MIN;
                rectTransform.sizeDelta = minSize;
                break;
            case SizeState.MIN:
                sizeState = SizeState.Default;
                rectTransform.sizeDelta = deafaultSize;
                break;
        }
    }

    new void SendMessage(string message) // InputField 에서 Enter 시
    {
        if (string.IsNullOrEmpty(message))
            return;

        Debug.Log("Publish to default channnel");
        chatClient.PublishMessage(curChannelName, new ChatData(userName, inputField.text, nickNameColor));
        //PhotonNetwork.LocalPlayer.SetMafiaReady(true);  <- 이거 뭐에 쓰는거임?
        KnifeGameManager.Instance.Player.photonView.RPC("OpenSpeechBubble", RpcTarget.All, userName, inputField.text);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void PublishMessage(ChatData chatdata)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        //chatClient.PublishMessage(curChannelName, chatdata);
        // 다른 채널에 킬/데스 메세지를 보내서 킬로그 시스템 구현? X
        //  => 킬로그는 이거 호출하는 대신 IOnEventCallback 방식으로 구현
    }


    #endregion
    /******************************************************
    *             IChatClientListener Callbacks
    ******************************************************/

    #region Callbacks
    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        Debug.Log($"[ChatState : {state}]");
    }

    void IChatClientListener.OnConnected()
    {
        // PublishSubscribers = true 가 아니면 OnuserSubscribe 가 콜백되지 않음
        chatClient.Subscribe(curChannelName, 0, -1,
            new ChannelCreationOptions() { PublishSubscribers = true, MaxSubscribers = PhotonNetwork.CurrentRoom.MaxPlayers });
    }

    void IChatClientListener.OnDisconnected()
    {
        Debug.Log("Chat Disconnected");
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //Debug.Log($"GetMessage from {channelName}");
        //chatClient.Service(); 가 이거 호출함
        // 일반 채널 채팅 수신
        if (channelName.Equals(curChannelName))
        {
            for (int i = 0; i < senders.Length; i++)
            {
                if (senders[i] == null)
                    return;
                ChatEntry newChat = Instantiate(chatEntry, contents);
                ChatData chatData = (ChatData) messages[i];
                newChat.SetChat(chatData);
            }
        }
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        //개인 메세지 수신
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        //내가 채널 입장시
        //ChatEntry newChat = Instantiate(chatEntry, contents);
       // newChat.SetChat(new ChatData(" ", $"You Entered Room:{curChannelName}", Color.black, Color.green));
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        //내가 채널에서 퇴장할 시
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
        //유저가 채널에 입장 할 시
        //ChatEntry newChat = Instantiate(chatEntry, contents);
       // newChat.SetChat(new ChatData(" ", $"{user} was Entered {channel}", Color.black, Color.blue));
    }

    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
        // 유저가 채널 퇴장시
    }

    #endregion
}
