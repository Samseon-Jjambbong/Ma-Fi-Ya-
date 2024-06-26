using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;


public class MafiaGameChatManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    static MafiaGameChatManager instance;
    static public MafiaGameChatManager Instance { get { return instance; } }

    [SerializeField] bool isChatable = true;
    public bool IsChatable
    {
        get { return isChatable; }
        set
        {
            if (!isMafia)
            {
                isChatable = value;
                inputField.interactable = isChatable;
            }
        }
    }

    [SerializeField] ChatEntry chatEntry;
    [SerializeField] Transform contents;
    [SerializeField] RectTransform rectTransform;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button buttonFixSize;

    private ChatClient chatClient;

    private string userName;
    private string curChannelName;
    private string mafiaChannelName;
    private string ghostChannelName;

    [Header("Message Color Settings")]
    [SerializeField] private Color nickNameColor;
    [SerializeField] Color mafiaMessageColor;
    [SerializeField] Color ghostMessageColor;

    [Header("For Debugging")]
    [SerializeField] bool isGhost; // = false; /////////////////////////////
    [SerializeField] public bool isMafia;
    [SerializeField] public bool isDay; // MafiaGameManager의 isDay에 이벤트 연결해서 사용할것.

    [SerializeField]
    public ChatData chatdata; 

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
        PhotonPeer.RegisterType(typeof(ChatData), (byte) 'C', ChatData.Serialize, ChatData.Deserialize);

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

        if (!isChatable)
            return;

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
        chatdata = new ChatData(userName);
        chatClient = new ChatClient(this);
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(userName);

        ChatEntry newChat = Instantiate(chatEntry, contents);
        newChat.SetChat(new ChatData(" ", $"Connect On Chatting Channel...", Color.black, Color.yellow));

        chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());

        curChannelName = PhotonNetwork.CurrentRoom.Name + "InGame";
        mafiaChannelName = curChannelName + "Night";
        ghostChannelName = curChannelName + "Ghots";

        Debug.Log($"PlayerRole: {PhotonNetwork.LocalPlayer.GetPlayerRole()}");
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
        if (!isChatable)
            return;

        if (string.IsNullOrEmpty(message))
            return;

        if (isGhost) //죽었으면 고스트 채널에
        {
            chatdata.message = inputField.text;
            chatClient.PublishMessage(ghostChannelName, chatdata);
            Manager.Mafia.Player.photonView.RPC("OpenSpeechBubble", RpcTarget.All, userName, inputField.text);
        }
        else if (!Manager.Mafia.IsDay && isMafia)
        //if(!MafiaManager.Instance.IsDay && isMafia ) // 낮이 아니고, 마피아라면 마피아 채널에
        {
            //Debug.Log("Publish to mafia channnel");
            chatdata.message = inputField.text;
            chatdata.messageColor = mafiaMessageColor;
            chatClient.PublishMessage(mafiaChannelName, chatdata);

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.GetPlayerRole().Equals(MafiaRole.Mafia))
                {
                    Manager.Mafia.House.photonView.RPC("OpenSpeechBubble", player, userName, inputField.text);
                }
            }
        }
        else // 이외라면 일반 채팅 채널에
        {
            //Debug.Log("Publish to default channnel");
            chatdata.messageColor = Color.black;
            chatdata.message = inputField.text;
            //Debug.Log($"{chatdata.nameColor.r},{chatdata.nameColor.g},{chatdata.nameColor.b}");
            chatClient.PublishMessage(curChannelName, chatdata);
            PhotonNetwork.LocalPlayer.SetMafiaReady(true);
            Manager.Mafia.Player.photonView.RPC("OpenSpeechBubble", RpcTarget.All, userName, inputField.text);
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void PublishMessage(ChatData newChatdata)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        chatClient.PublishMessage(curChannelName, newChatdata);
    }

    public void SubscribleGhostChannel()
    {
        chatClient.Subscribe(ghostChannelName, 0, 0);
        chatdata.messageColor = ghostMessageColor;
        isGhost = true;
    }

    #endregion
    /******************************************************
    *              MonoBehaviourPunCallbacks Callbacks
    ******************************************************/


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.PLAYERCOLOR))
        {
            //nickNameColor = changedProps[CustomProperty.PLAYERCOLOR];
            //nickNameColor = PhotonNetwork.LocalPlayer.GetPlayerColor();
            //Debug.Log($"{nickNameColor.r},{nickNameColor.g},{nickNameColor.b}");
            //chatdata.nameColor = nickNameColor;
        }

        if (changedProps.ContainsKey(CustomProperty.PLAYERROLE))
        {
            isMafia = (PhotonNetwork.LocalPlayer.GetPlayerRole() == MafiaRole.Mafia);
        }
    }

    /******************************************************
    *              IChatClientListener Callbacks
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
        if (isMafia)
        {
            chatClient.Subscribe(mafiaChannelName);
        }
    }

    void IChatClientListener.OnDisconnected()
    {
        Debug.Log("Chat Disconnected");
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log($"GetMessage from {channelName}");
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
                Debug.Log($"{chatData.name} Send {chatData.message}");
                newChat.SetChat(chatData);
            }
        }

        // 마피아일 때만 마피아 채널의 메세지 수신
        if (isMafia && channelName.Equals(mafiaChannelName))
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

        //죽었을 때는 ghost 채널 메세지 수신
        if (isGhost && channelName.Equals(ghostChannelName))
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
        //newChat.SetChat(new ChatData(" ", $"You Entered Room:{curChannelName}", Color.black, Color.green));
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        //내가 채널에서 퇴장할 시
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
        //유저가 채널에 입장 할 시
        ChatEntry newChat = Instantiate(chatEntry, contents);
        newChat.SetChat(new ChatData(" ", $"{user} was Entered {channel}", Color.black, Color.blue));
    }

    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
        // 유저가 채널 퇴장시
    }
    #endregion
}
