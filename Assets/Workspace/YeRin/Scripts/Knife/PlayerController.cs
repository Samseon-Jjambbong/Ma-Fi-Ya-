using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour//Pun
{
    [Header("Components")]
    //[SerializeField] TMP_Text nickNameText;
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

    private Vector3 moveDir;

    private void FixedUpdate()
    {
        /*if (photonView.IsMine)
        {
            Accelate();
        }*/

        Accelate();
    }

    private void Update()
    {
        /*if (photonView.IsMine)
        {
            Rotate();
        }*/
        Rotate();
    }

    private void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;

        /*if (photonView.IsMine)
        {
            photonView.RPC("Walk", RpcTarget.All);
        }*/

        WalkStart();
    }

    private void Accelate()
    {
        /*if (moveDir.x == 0 && moveDir.z == 0 && isWalking)
        {
            //photonView.RPC("Walk", RpcTarget.All);
            Walk();
        }*/

        if (controller.velocity == Vector3.zero && isWalking)
        {
            WalkStop();
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
        /*if (photonView.IsMine)
            photonView.RPC("HipHop", RpcTarget.All);*/
        HipHop();
    }

    [PunRPC]
    private void OnRumbaDance(InputValue value)
    {
        /*if (photonView.IsMine)
            photonView.RPC("Rumba", RpcTarget.All);*/
        Rumba();
    }

    private void OnSillyDance(InputValue value)
    {
        /*if (photonView.IsMine)
            photonView.RPC("Silly", RpcTarget.All);*/
        Silly();
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
}
