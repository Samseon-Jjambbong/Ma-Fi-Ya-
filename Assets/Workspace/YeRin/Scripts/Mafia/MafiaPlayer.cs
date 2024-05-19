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

    [Header("Movement Values")]
    [SerializeField] float movePower;
    [SerializeField] float maxSpeed;
    [SerializeField] float rotateSpeed;
    private Vector3 moveDir;
    private float currentSpeed;

    [Header("Properties")]
    private bool isAlive = true; // 플레이어의 생존 여부
    public bool IsAlive { get { return isAlive; } }

    [Header("Mafia Logic")]
    private Dictionary<int, Player> playerDic;
    public MafiaActionType actionType;
    public MafiaAction? actionByThisPlayer = null;
    public List<MafiaActionType> actionsOnThisPlayer = new List<MafiaActionType>();

    [Header("States")]
    private bool skillBlocked;
    private bool isWalking;
    public bool IsMine => photonView.IsMine;
    int ID => PhotonNetwork.LocalPlayer.ActorNumber;
    int Idx => PhotonNetwork.LocalPlayer.ActorNumber - 1;

    protected virtual void Start()
    {
        // 플레이어 역할 받기
        playerDic = PhotonNetwork.CurrentRoom.Players;

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
                    actionType = (MafiaActionType) Random.Range(0, 4);
                    break;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetColor", RpcTarget.MasterClient, Color.black.r, Color.black.g, Color.black.b);
        }
        walkAudio.Stop();
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

    #region Game Logic
    [PunRPC]
    public void AddAction(int[] serialized)
    {
        MafiaAction action = new MafiaAction(serialized);
        Debug.Log($"Add Action RPC Info: {action.sender} {action.receiver} {action.actionType}");
        if (PhotonNetwork.LocalPlayer.ActorNumber == action.sender)
        {
            actionByThisPlayer = action;
            Debug.Log($"{GetRole()} targeted Player{action.receiver}");
        }
        if (PhotonNetwork.LocalPlayer.ActorNumber == action.receiver)
        {
            if (action.actionType == MafiaActionType.Block)
                skillBlocked = true;

            actionsOnThisPlayer.Add(action.actionType);
            Debug.Log($"{action.receiver} got {action.actionType}ed");
        }
    }

    public IEnumerator ShowActionsRoutine()
    {
        if (actionByThisPlayer != null)
        {
            MafiaAction action = (MafiaAction) actionByThisPlayer;
            Debug.Log($"Player{ID} did {action.actionType}");
            yield return Manager.Mafia.PlayerGoRoutine(action);
        }
        else
        {
            Debug.Log("actionby null");
        }

        yield return new WaitForSeconds(1);

        foreach (MafiaActionType actionType in actionsOnThisPlayer)
        {
            House house = Manager.Mafia.Houses[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            Debug.Log($"Player{ID} received {actionType}");
            yield return Manager.Mafia.PlayerComeRoutine(house, actionType);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        
        actionByThisPlayer = null;
        actionsOnThisPlayer.Clear();
        Manager.Mafia.nightEventFinishedCount++;
    }

    [PunRPC]
    public void DieAnimation(int playerID)
    {
        //죽은 플레이어의 죽는 모션 보여주기
    }
    #endregion

    #region Photon
    public void SetPlayerHouse(int playerNumber)
    {
        photonView.RPC("AddHouseList", RpcTarget.All, playerNumber);
        Manager.Mafia.Houses[playerNumber].ActivateOutline(false);
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

    [PunRPC]
    private void NickName(string nickName)
    {
        nickNameText.text = nickName;
    }
    #endregion
}