using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun
{
    [Header("Components")]
    [SerializeField] TMP_Text nickNameText;
    //[SerializeField] Rigidbody rigid;
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource walkAudio;

    //[SerializeField] GameObject speechBubble;
    //[SerializeField] TMP_Text bubbleText;

    [SerializeField] float movePower;
    [SerializeField] float rotateSpeed;

    [Header("States")]
    [SerializeField] private bool isWalking;

    [Header("Knife")]
    [SerializeField] GameObject shortKnife;
    [SerializeField] GameObject middleKnife;
    [SerializeField] GameObject longKnife;

    private Vector3 moveDir;

    private void Start()
    {
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

    private void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;

        if (photonView.IsMine)
        {
            photonView.RPC("WalkStart", RpcTarget.All);
        }
    }

    private void Accelate()
    {

        if (moveDir.x == 0 && moveDir.z == 0 && isWalking)
        {
            photonView.RPC("WalkStop", RpcTarget.All);
        }

        controller.Move(transform.forward * moveDir.z * movePower * Time.deltaTime);
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
    private void WalkStart()
    {
        animator.Play("Walk");
        isWalking = true;
        walkAudio.Play();
    }

    private void WalkStop()
    {
        animator.Play("Idle");
        isWalking = false;
        walkAudio.Stop();
    }

    [PunRPC]
    private void OnAttack()
    {
        if (photonView.IsMine)
            Attack();
    }

    private void Attack()
    {
        if (shortKnife.activeSelf)
        {
            animator.SetTrigger($"shortAttack{Random.Range(1, 3)}");
        }
        else if (middleKnife.activeSelf)
        {
            animator.SetTrigger($"middleAttack{Random.Range(1, 3)}");
        }
        else if (longKnife.activeSelf)
        {
            animator.SetTrigger($"longAttack{Random.Range(1, 3)}");
        }
        else
        {
            Debug.Log("No Weapon");
        }
    }

    private void HipHop()
    {
        animator.SetTrigger("hipHop");
    }

    private void Rumba()
    {
        animator.SetTrigger("rumba");
    }

    private void Silly()
    {
        animator.SetTrigger("silly");
    }

    [PunRPC]
    private void NickName(string nickName)
    {
        nickNameText.text = nickName;
    }

    public void SetNickName(string nickName)   // PhotonNetwork.PlayerList[playerNumber].NickName
    {
        photonView.RPC("NickName", RpcTarget.All, nickName);
    }
}
