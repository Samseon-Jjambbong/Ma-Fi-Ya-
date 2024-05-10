using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private List<Color> colorList;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotateSpeed;
    
    [SerializeField] private int fireCount;

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float fireCoolTime;
    private Rigidbody rb;
    private PlayerInput playerInput;
    
    private Vector3 inputDir;
    private float currentSpeed;
    private float lastFireTime = float.MinValue;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        
        // Prevent control over remote players
        if (!photonView.IsMine)
            Destroy(GetComponent<PlayerInput>());
        
        // Assign color based on id
        SetPlayerColor();
    }

    private void Update()
    {
        Accelerate(inputDir.y);
        Rotate(inputDir.x);
    }

    private void OnMove(InputValue value)
    {
        inputDir = value.Get<Vector2>();
    }

    private void Accelerate(float input)
    {
        rb.AddForce(input * moveSpeed * transform.forward, ForceMode.Force);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void Rotate(float input)
    {
        transform.Rotate(Vector3.up, input * rotateSpeed * Time.deltaTime);
    }

    private void OnFire()
    {
        photonView.RPC("RequestCreateBullet", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RequestCreateBullet()
    {
        if (Time.time < lastFireTime + fireCoolTime)
            return;

        lastFireTime = Time.time;
        photonView.RPC("ResultCreateBullet", RpcTarget.AllViaServer, transform.position, transform.rotation);
    }

    [PunRPC]
    private void ResultCreateBullet(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        Bullet bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.transform.position += bullet.Velocity * lag;
        fireCount++;
    }
    
    private void SetPlayerColor()
    {
        int playerNumber = photonView.Owner.GetPlayerNumber();
        if (colorList == null || colorList.Count <= playerNumber)
            return;

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = colorList[playerNumber];

        if (photonView.IsMine)
        {
            renderer.material.color = Color.green;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // photonView.IsMine
        {
            stream.SendNext(fireCount);
        }
        else // !photonView.IsMine
        {
            fireCount = (int)stream.ReceiveNext();
        }
    }
}
