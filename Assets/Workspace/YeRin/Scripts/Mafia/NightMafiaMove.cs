using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// programmer : Yerin
/// 
/// A character who will move at night 
/// </summary>

public class NightMafiaMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    //[SerializeField] GameObject target;

    [SerializeField] AudioSource walkAudio;
    //public GameObject Target { get { return target; } set { target = value; } }

    [SerializeField] Animator animator;

    private void Start()
    {
        walkAudio.Stop();
    }

    Vector3 targetPos;
    public IEnumerator MoveToTargetHouse(House house)
    {
        Transform target = house.entrance;
        targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position).normalized;

        animator.Play("Walk");
        walkAudio.Play();

        while ((int)transform.position.x != (int)targetPos.x)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public IEnumerator DieAnimation()
    {
        animator.Play("Die");
        yield return new WaitForSeconds(3);
    }

    //public IEnumerator MoveToTargetPos(Vector3 target)
    //{
    //    targetPos = new Vector3(target.x, transform.position.y, target.z);
    //    animator.Play("Walk");
    //    walkAudio.Play();

    //}

    //public void MoveToTarget()
    //{
    //    StartCoroutine(MoveToTargetHouse());
    //}
}
