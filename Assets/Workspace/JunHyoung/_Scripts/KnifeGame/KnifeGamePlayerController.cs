using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnifeGamePlayerController : MonoBehaviourPun, IPunObservable
{
    [Header("Component")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;


    [Header("Spec")]
    [SerializeField] float moveSpeed;
    [SerializeField] float walkSpeed;

    private Vector3 moveDir;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    /******************************************************
    *                    Unity Events
    ******************************************************/

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /******************************************************
    *                    Input Actions
    ******************************************************/

    private void OnMove(InputValue inputValue)
    {
        Vector2 inputDir = inputValue.Get<Vector2>();
        moveDir.x = inputDir.x;
        moveDir.z = inputDir.y;
    }



}
