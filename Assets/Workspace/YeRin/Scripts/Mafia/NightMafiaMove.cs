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
    [SerializeField] GameObject target;

    [SerializeField] AudioSource walkAudio;
    public GameObject Target { get { return target; } set { target = value; } }

    [SerializeField] Animator animator;

    private void Start()
    {
        walkAudio.Stop();
    }

    Vector3 targetPos;

    IEnumerator MoveToTargetHouse()
    {
        Vector3 pos = new Vector3(target.transform.position.z, 0, target.transform.position.x);
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

    public void MoveToTarget()
    {
        StartCoroutine(MoveToTargetHouse());
    }
}
