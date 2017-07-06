using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow Instance;

    public List<Transform> targets;

    [Space()]
    public float height = 15.0f;
    public float zOffset = -6.5f;

    [Space()]
    public float followSpeed = 10.0f;

	void Awake()
	{
		Instance = this;
	}

    void LateUpdate()
    {
		Vector3 targetPos = Vector3.zero;

		if (targets.Count > 0)
		{
			foreach (Transform target in targets)
			{
				targetPos += target.position;
			}

			targetPos /= targets.Count;
		}

        targetPos += Vector3.up * height;
        targetPos += Vector3.forward * zOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
