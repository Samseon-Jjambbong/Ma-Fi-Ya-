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
    [SerializeField] float jumpForce;
    Rigidbody rb;
    //[SerializeField] GameObject target;

    [SerializeField] AudioSource walkAudio;
    //public GameObject Target { get { return target; } set { target = value; } }

    [SerializeField] Animator animator;

    private void Start()
    {
        walkAudio.Stop();
        rb = GetComponent<Rigidbody>();
    }

    public IEnumerator Fly()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    Vector3 targetPos;
    public IEnumerator MoveToTargetHouse(House house)
    {
        Transform target = house.entrance;
        targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position).normalized;

        animator.Play("Walk");
        walkAudio.Play();

        while ((targetPos - transform.position).magnitude > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator DieAnimation()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(3);
    }

    public IEnumerator MoveToTargetPos(Vector3 target)
    {
        targetPos = new Vector3(target.x, transform.position.y, target.z);
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position).normalized;
        animator.Play("Walk");
        walkAudio.Play();

        while ((targetPos - transform.position).magnitude > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        animator.Play("Idle");
    }

    //public void MoveToTarget()
    //{
    //    StartCoroutine(MoveToTargetHouse());
    //}
}
