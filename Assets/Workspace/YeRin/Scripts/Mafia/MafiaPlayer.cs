using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [SerializeField] float movePower;
    [SerializeField] float maxSpeed;
    [SerializeField] float rotateSpeed;

    // 플레이어의 생존 여부
    private bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }

    private Dictionary<int, Player> playerDic;

    // Tae Player Logic
    public MafiaActionType actionType;
    public int targetPlayerID;
    public MafiaActionPQ actionsOnThisPlayer = new MafiaActionPQ();
    
    private Vector3 moveDir;
    private float currentSpeed;

    private bool isWalking;

    protected virtual void Start()
    {
        // 플레이어 역할 받기
        playerDic = PhotonNetwork.CurrentRoom.Players;


        if ( PhotonNetwork.IsMasterClient )
        {
            photonView.RPC("SetColor", RpcTarget.MasterClient, Color.black.r, Color.black.g, Color.black.b);
        }
        walkAudio.Stop();
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

    [PunRPC]
    public void ShowNightResults()
    {
        Debug.Log($"Player{PhotonNetwork.LocalPlayer.ActorNumber} Results. Count: {actionsOnThisPlayer.Count}");
        while(actionsOnThisPlayer.Count > 0)
        {
            MafiaAction action = actionsOnThisPlayer.Dequeue();
            Debug.Log($"Action on this player: {action.actionType}");
        }
    }

    #region Game Logic
    [PunRPC]
    public void OnChooseTarget(int[] serialized)
    {
        // Deserialize Action
        MafiaAction action = new MafiaAction(serialized);
        Debug.Log($"sender: {action.sender}");
        Debug.Log($"receiver: {action.receiver}");
        Debug.Log($"action: {action.actionType}");

        if(PhotonNetwork.LocalPlayer.ActorNumber == action.sender)
        {
            targetPlayerID = action.receiver;
            Debug.Log($"{GetRole()} targeted Player{targetPlayerID}");
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber == action.receiver)
        {
            actionsOnThisPlayer.Enqueue(action);
            Debug.Log($"{action.receiver} got {action.actionType}ed");
        }
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
}