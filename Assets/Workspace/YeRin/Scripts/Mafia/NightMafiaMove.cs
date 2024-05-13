using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// programmer : Yerin
/// 
/// A character who will move at night 
/// </summary>

public class NightMafiaMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject target;
    public GameObject Target { get { return target; } set { target = value; } }

    [SerializeField] Animator animator;

    private bool isActive;
    public bool ISActive => isActive;


    Vector3 targetPos;
    private void OnEnable()
    {
        isActive = true;
    }

    private void OnDisable()
    {
        isActive = false;
    }

    IEnumerator MoveToTargetHouse()
    {
        Vector3 pos = new Vector3(target.transform.position.z, 0, target.transform.position.x);
        targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position).normalized;

        animator.Play("Walk");

        while ( transform.position != targetPos )
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed);
            yield return null;
        }

        animator.Play("Idle");
    }

    public void MoveToTarget()
    {
        StartCoroutine(MoveToTargetHouse());
    }
}
