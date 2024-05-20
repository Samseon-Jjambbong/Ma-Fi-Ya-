using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

/// <summary>
/// programmer : Yerin, TaeHong
/// 
/// Class for Mafia mode players
/// </summary>
public class MafiaPlayer : MonoBehaviourPun
{
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
    [SerializeField] private bool isAlive;// = false; /////////////////////////////
    public bool IsAlive { get { return isAlive; } }

    private Dictionary<int, Player> playerDic;

    // Tae Player Logic
    public MafiaActionType actionType;
    public MafiaAction actionByThisPlayer;
    public MafiaActionPQ actionsOnThisPlayer = new MafiaActionPQ();

    private bool skillBlocked;
    public bool IsMine => photonView.IsMine;

    private Vector3 moveDir;
    private float currentSpeed;

    private bool isWalking;

    Coroutine bubble;

    protected virtual void Start()
    {
        // 플레이어 역할 받기
        playerDic = PhotonNetwork.CurrentRoom.Players;


        if ( PhotonNetwork.IsMasterClient )
        {
            photonView.RPC("SetColor", RpcTarget.MasterClient, Color.black.r, Color.black.g, Color.black.b);
        }
        walkAudio.Stop();

        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerRole());
    }

    private void FixedUpdate()
    {
        if ( photonView.IsMine )
        {
            Accelate();
        }
    }

    private void Update()
    {
        if ( photonView.IsMine )
        {
            Rotate();
        }
    }

    public MafiaRole GetRole()
    {
        return PhotonNetwork.LocalPlayer.GetPlayerRole();
    }

    #region Game Logic
    [PunRPC] // 집을 선택시 모든 플레이어에게 정보를 보낸다
    public void OnChooseTarget(int[] serialized)
    {
        // Deserialize Action
        MafiaAction action = new MafiaAction(serialized);
        Debug.Log($"sender: {action.sender}");
        Debug.Log($"receiver: {action.receiver}");
        Debug.Log($"action: {action.actionType}");

        if(PhotonNetwork.LocalPlayer.ActorNumber == action.sender)
        {
            actionByThisPlayer = action;
            Debug.Log($"{GetRole()} targeted Player{action.receiver}");
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber == action.receiver)
        {
            if (action.actionType == MafiaActionType.Block)
                skillBlocked = true;

            actionsOnThisPlayer.Enqueue(action);
            Debug.Log($"{action.receiver} got {action.actionType}ed");
        }
    }

    [PunRPC] // 플레이어랑 관련된 모든 행동들을 화면에 보여준다
    public void ShowNightResults()
    {
        PhotonNetwork.LocalPlayer.SetMafiaReady(false);

        bool playerDies = false; //결국에 죽는지

        Debug.Log($"Player{PhotonNetwork.LocalPlayer.ActorNumber} Results. Count: {actionsOnThisPlayer.Count}");
        while (actionsOnThisPlayer.Count > 0)
        {
            MafiaAction action = actionsOnThisPlayer.Dequeue();
            Debug.Log($"Action on this player: {action.actionType}");

            switch (action.actionType)
            {
                case MafiaActionType.Block:
                    // BlockAction();
                    break;
                case MafiaActionType.Kill:
                    // KillAction();
                    playerDies = true;
                    break;
                case MafiaActionType.Heal:
                    // HealAction();
                    if (playerDies)
                    {
                        //힐 효과 생성
                    }
                    playerDies = false;
                    break;
            }
        }

        if (playerDies)
        {
            // 죽었다
            Debug.Log("Player died");
            Manager.Mafia.GetComponent<PhotonView>().RPC("PlayerDied", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        PhotonNetwork.LocalPlayer.SetMafiaReady(true);
    }

    [PunRPC]
    public void DieAnimation(int playerID)
    {
        //죽은 플레이어의 죽는 모션 보여주기
    }
    #endregion

    #region Photon
    public void SetPlayerHouse( int playerNumber )
    {
        photonView.RPC("AddHouseList", RpcTarget.All, playerNumber);
        Manager.Mafia.Houses [playerNumber].ActivateOutline(false);
    }

    private void OnMove( InputValue value )
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;

        if ( photonView.IsMine )
        {
            photonView.RPC("Walk", RpcTarget.All);
        }
    }

    private void Accelate()
    {
        if ( moveDir.x == 0 && moveDir.z == 0 && isWalking )
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

        if ( rigid.velocity.sqrMagnitude > maxSpeed * maxSpeed )
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
    private void OnHipHopDance( InputValue value)
    {
        if ( photonView.IsMine )
            photonView.RPC("HipHop", RpcTarget.All);
    }

    [PunRPC]
    private void OnRumbaDance( InputValue value )
    {
        if ( photonView.IsMine )
            photonView.RPC("Rumba", RpcTarget.All);
    }

    private void OnSillyDance( InputValue value )
    {
        if ( photonView.IsMine )
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
    private void AddHouseList( int playerNumber )
    {
        Manager.Mafia.Houses [playerNumber].HouseOwner = this;
    }

    public void SetNickName(string nickName)
    {
        photonView.RPC("NickName", RpcTarget.All, nickName);
    }

    [PunRPC]
    private void NickName( string nickName )
    {
        nickNameText.text = nickName;
    }
    #endregion
    
    [PunRPC]
    private void SetColor(float r, float g, float b)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponentInChildren<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1f);
            Color color = GetComponentInChildren<Renderer>().material.color;
            photonView.RPC("SetColor", RpcTarget.Others, color.r, color.g, color.b);
            return;
        }
        GetComponentInChildren<Renderer>().material.color = new Color(r, g, b, 1f);
    }

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