using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Programmer : Yerin
/// 
/// About player control in Knife Game Mode
/// </summary>
public class PlayerController : MonoBehaviourPun
{
    [Header("Components")]
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource walkAudio;
    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject speechBubble;
    [SerializeField] TMP_Text bubbleText;
    [SerializeField] LayerMask deathZone;
    [SerializeField] SpriteRenderer flatArrow;

    public TMP_Text Name => nickNameText;

    [SerializeField] Vector3 playerSpawnPos;
    [SerializeField] float movePower;
    [SerializeField] float rotateSpeed;

    [Header("States")]
    [SerializeField] private bool isWalking;
    [SerializeField] private bool canMove;
    public bool CanMove { get { return canMove; } set { canMove = value; } }

    [Header("Knife")]
    [SerializeField] GameObject shortKnife;
    [SerializeField] GameObject middleKnife;
    [SerializeField] GameObject longKnife;

    [SerializeField] LayerMask layerMask;
    [SerializeField] float range;
    [SerializeField, Range(0, 360)] float angle;

    private float preAngle;
    private float cosAngle;
    private float CosAngle;

    Collider[] colliders = new Collider[20];

    private Vector3 moveDir;
    Coroutine bubble;

    private void Start()
    {
        walkAudio.Stop();
        SetWeaponLength();
        playerSpawnPos = transform.position;

        if (photonView.IsMine)
        {
            flatArrow.color = new Color(0, 255, 0);
        }
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

    #region Move
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
        transform.Rotate(Vector3.up, moveDir.x * rotateSpeed * 100f * Time.deltaTime);
    }

    [PunRPC]
    private void WalkStart()
    {
        animator.Play("Walk");
        isWalking = true;
        walkAudio.Play();
    }

    [PunRPC]
    private void WalkStop()
    {
        animator.Play("Idle");
        isWalking = false;
        walkAudio.Stop();
    }
    #endregion

    #region Dance
    private void OnHipHopDance(InputValue value)
    {
        if (photonView.IsMine)
            photonView.RPC("HipHop", RpcTarget.All);
    }

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
    #endregion

    #region Attack
    private void SetWeaponLength()
    {
        if (shortKnife.activeSelf)
        {
        }
        else if (middleKnife.activeSelf)
        {
            range = range * 2;
        }
        else if (longKnife.activeSelf)
        {
            range = range * 3;
        }
        else
        {
            Debug.Log("No Weapon");
        }
    }
    private void OnAttack()
    {
        if (photonView.IsMine)
            photonView.RPC("Attack", RpcTarget.All);
    }

    [PunRPC]
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

        AttackRange();
    }

    private void AttackRange()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, range, colliders, layerMask);
        for (int i = 0; i < size; i++)
        {
            Vector3 dirToTarget = (colliders[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, dirToTarget) < CosAngle)
                continue;

            PlayerController player = colliders[i].GetComponent<PlayerController>();
            if (player.gameObject == gameObject)
            {
                continue;
            }
            // 바로 죽음
            Debug.Log($"{player.Name.text} die");
            player.photonView.RPC("Die", RpcTarget.All);

            PhotonNetwork.LocalPlayer.AddPlayerKillCount();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    #endregion

    #region Die
    [PunRPC]
    private void Die()
    {
        StartCoroutine(DieState());
    }

    IEnumerator DieState()
    {
        controller.enabled = false;
        yield return new WaitForSeconds(1f);

        playerModel.SetActive(false);
        PhotonNetwork.LocalPlayer.AddPlayerDeathCount();
        transform.position = playerSpawnPos;

        yield return new WaitForSeconds(3f);

        controller.enabled = true;
        playerModel.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (deathZone.Contain(other.gameObject.layer))
        {
            playerModel.SetActive(false);
            photonView.RPC("Die", RpcTarget.All);
        }
    }
    #endregion

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

        bubbleText.text = $"<#333333>{userName}</color>\n<#666666>{sendText}</color>";

        bubble = StartCoroutine(CloseSpeechBubble());
    }
    IEnumerator CloseSpeechBubble()
    {
        yield return new WaitForSeconds(3f);

        speechBubble.SetActive(false);
    }
    #endregion

    #region etx
    [PunRPC]
    private void NickName(string nickName)
    {
        nickNameText.text = nickName;
    }

    public void SetNickName(string nickName)
    {
        photonView.RPC("NickName", RpcTarget.All, nickName);
    }

    [PunRPC]
    private void SetWeapon(KnifeLength length)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            switch (Random.Range(0, 3))
            {
                case 1:
                    shortKnife.gameObject.SetActive(true);
                    length = KnifeLength.Short;
                    break;
                case 2:
                    middleKnife.gameObject.SetActive(true);
                    length = KnifeLength.Middle;
                    break;
                case 3:
                    longKnife.gameObject.SetActive(true);
                    length = KnifeLength.Long;
                    break;
            }

            photonView.RPC("SetWeapon", RpcTarget.Others, length);
            return;
        }

        switch (length)
        {
            case KnifeLength.Short:
                shortKnife.gameObject.SetActive(true);
                break;
            case KnifeLength.Middle:
                middleKnife.gameObject.SetActive(true);
                break;
            case KnifeLength.Long:
                longKnife.gameObject.SetActive(true);
                break;
        }
    }
    #endregion
}
