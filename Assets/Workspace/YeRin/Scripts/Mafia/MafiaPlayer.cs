using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Class for Mafia mode players
/// </summary>
public class MafiaPlayer : MonoBehaviourPun
{
    [Header("Components")]
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource walkAudio;

    [SerializeField] GameObject speechBubble;
    [SerializeField] TMP_Text bubbleText;

    [SerializeField] float movePower;
    [SerializeField] float maxSpeed;
    [SerializeField] float rotateSpeed;

    // 플레이어의 생존 여부
    private Vector3 moveDir;
    private float currentSpeed;

    [Header("Properties")]
    private bool isAlive = true; // 플레이어의 생존 여부
    public bool IsAlive { get { return isAlive; } }

    [Header("Mafia Logic")]
    private Dictionary<int, Player> playerDic;
    public MafiaActionType actionType;
    public MafiaRole fakeRole; // used only by insane

    [Header("States")]
    private bool skillBlocked;
    private bool isWalking;
    public bool IsMine => photonView.IsMine;
    public int ID => PhotonNetwork.LocalPlayer.ActorNumber;
    public int Idx => PhotonNetwork.LocalPlayer.ActorNumber - 1;

    Coroutine bubble;
    [SerializeField]
    Color curColor;

    protected virtual void Start()
    {
        // 플레이어 역할 받기
        playerDic = PhotonNetwork.CurrentRoom.Players;

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetColor", RpcTarget.MasterClient, Color.black.r, Color.black.g, Color.black.b);
        }

        if (IsMine)
        {
            switch (PhotonNetwork.LocalPlayer.GetPlayerRole())
            {
                case MafiaRole.Mafia:
                    actionType = MafiaActionType.Kill;
                    break;
                case MafiaRole.Doctor:
                    actionType = MafiaActionType.Heal;
                    break;
                case MafiaRole.Police:
                    actionType = MafiaActionType.Block;
                    break;
                case MafiaRole.Insane:
                    float random = Random.Range(0f, 1f);
                    if(random <= 0.5f)
                    {
                        actionType = MafiaActionType.Block;
                        fakeRole = MafiaRole.Police;
                    }
                    else
                    {
                        actionType = MafiaActionType.Heal;
                        fakeRole = MafiaRole.Doctor;
                    }
                    break;
            }
        }

        walkAudio.Stop();

        if (photonView.IsMine)
        {
            Manager.Mafia.ShowRoleList();
        }
        
        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerRole());
    }

    private void OnEnable()
    {

    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Accelate();
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Rotate();
        }
    }

    public MafiaRole GetRole()
    {
        return PhotonNetwork.LocalPlayer.GetPlayerRole();
    }

    /******************************************************
    *                    Game Logic
    ******************************************************/
    #region Game Logic
    public IEnumerator ShowActionsRoutine()
    {
        MafiaAction? actionByThisPlayer = null;
        List<MafiaActionType> actionsOnThisPlayer = new List<MafiaActionType>();

        // Get actions
        if (Manager.Mafia.sharedData.sentActionDic.ContainsKey(ID))
        {
            actionByThisPlayer = Manager.Mafia.sharedData.sentActionDic[ID];
        }
        if (Manager.Mafia.sharedData.receivedActionDic.ContainsKey(ID))
        {
            actionsOnThisPlayer = Manager.Mafia.sharedData.receivedActionDic[ID];
        }

        // Do Actions
        if (actionByThisPlayer != null)
        {
            MafiaAction action = (MafiaAction) actionByThisPlayer;
            Debug.Log($"Player{ID} did {action.actionType}");
            yield return Manager.Mafia.animFactory.PlayerGoActionRoutine(action);
        }

        yield return new WaitForSeconds(1);

        foreach (MafiaActionType actionType in actionsOnThisPlayer)
        {
            Debug.Log($"Player{ID} received {actionType}");
            yield return Manager.Mafia.animFactory.PlayerComeActionRoutine(ID, actionType);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        Manager.Mafia.sharedData.photonView.RPC("ClientFinished", RpcTarget.All);
    }

    [PunRPC]
    public void DieAnimation(int playerID)
    {
        //죽은 플레이어의 죽는 모션 보여주기
    }
    #endregion

    /******************************************************
    *                    Photon
    ******************************************************/
    #region Photon
    public void SetPlayerHouse(int playerNumber)
    {
        photonView.RPC("AddHouseList", RpcTarget.All, playerNumber);
        Manager.Mafia.Houses[playerNumber].DeactivateOutline();
    }

    private void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;

        if (photonView.IsMine)
        {
            photonView.RPC("Walk", RpcTarget.All);
        }
    }

    private void Accelate()
    {
        if (moveDir.x == 0 && moveDir.z == 0 && isWalking)
        {
            photonView.RPC("Walk", RpcTarget.All);
        }

        if (moveDir.z < 0)
        {
            rigid.AddForce(moveDir.z * transform.forward * (movePower + 100f), ForceMode.Force);
        }
        else
        {
            rigid.AddForce(moveDir.z * transform.forward * movePower, ForceMode.Force);
        }

        if (rigid.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }
        currentSpeed = rigid.velocity.magnitude;
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, moveDir.x * rotateSpeed * Time.deltaTime);
    }

    [PunRPC]
    private void OnHipHopDance(InputValue value)
    {
        if (photonView.IsMine)
            photonView.RPC("HipHop", RpcTarget.All);
    }

    [PunRPC]
    private void OnRumbaDance(InputValue value)
    {
        if (photonView.IsMine)
            photonView.RPC("Rumba", RpcTarget.All);
    }

    private void OnSillyDance(InputValue value)
    {
        if (photonView.IsMine)
            photonView.RPC("Silly", RpcTarget.All);
    }

    [PunRPC]
    private void Walk()
    {
        if (!isWalking)
        {
            animator.Play("Walk");
            isWalking = true;

            walkAudio.Play();
        }
        else
        {
            animator.Play("Idle");
            isWalking = false;

            walkAudio.Stop();
        }
    }

    [PunRPC]
    private void HipHop()
    {
        animator.SetTrigger("hipHop");
    }

    [PunRPC]
    private void Rumba()
    {
        animator.SetTrigger("rumba");
    }

    [PunRPC]
    private void Silly()
    {
        animator.SetTrigger("silly");
    }

    [PunRPC]
    private void AddHouseList(int playerNumber)
    {
        Manager.Mafia.Houses[playerNumber].HouseOwner = this;
    }

    public void SetNickName(string nickName)
    {
        photonView.RPC("NickName", RpcTarget.All, nickName);
    }

    [PunRPC]
    private void NickName(string nickName)
    {
        nickNameText.text = nickName;
    }

    [PunRPC]
    private void SetColor(float r, float g, float b)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponentInChildren<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1f);
            curColor = GetComponentInChildren<Renderer>().material.color;
            if (IsMine)
            {
                MafiaGameChatManager.Instance.chatdata.nameColor = curColor;
            }
            photonView.RPC("SetColor", RpcTarget.Others, curColor.r, curColor.g, curColor.b);
            return;
        }
        curColor = new Color(r, g, b, 1f);
        GetComponentInChildren<Renderer>().material.color = curColor;
        if (IsMine)
        {
            MafiaGameChatManager.Instance.chatdata.nameColor = curColor;
        }
        //PhotonNetwork.LocalPlayer.SetPlayerColor(new Color(r, g, b, 1f));
    }
    #endregion

    /******************************************************
    *                    Speech Bubble
    ******************************************************/
    #region Speech Bubble

    [PunRPC]
    public void OpenSpeechBubble(string userName, string sendText)
    {
        if (speechBubble.activeSelf)
        {
            StopCoroutine(bubble);
        }
        else
        {
            speechBubble.SetActive(true);
        }

        bubbleText.text = $"<#00C8FF>{userName}</color>\n{sendText}";

        bubble = StartCoroutine(CloseSpeechBubble());
    }
    IEnumerator CloseSpeechBubble()
    {
        yield return new WaitForSeconds(3f);

        speechBubble.SetActive(false);
    }

    #endregion
}