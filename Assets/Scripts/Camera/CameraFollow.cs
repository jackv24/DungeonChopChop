using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Space()]
    public float height = 15.0f;
    public float zOffset = -6.5f;

    [Space()]
    public float followSpeed = 10.0f;

    void Start()
    {
        if(!target)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player)
                target = player.transform;
        }
    }

    void LateUpdate()
    {
        if(target)
        {
            Vector3 targetPos = target.position;
            targetPos += Vector3.up * height;
            targetPos += Vector3.forward * zOffset;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }
}
