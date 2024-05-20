using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    [SerializeField] float rotateSpeed;

    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }

    //transform.RotateAround(Target.position, Vector3.up, rotateSpeed * Time.deltaTime); 특정 타겟 기준으로 회전
}
