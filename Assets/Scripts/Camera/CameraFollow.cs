using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow Instance;

    public List<Transform> players;

	private Vector3 max;
	private Vector4 min;

	[Space()]
	public float paddingTop = 2.0f;
	public float paddingBottom = 2.0f;
	public float paddingLeft = 2.0f;
	public float paddingRight = 2.0f;

	[Space()]
    public float height = 15.0f;
    public float zOffset = -6.5f;

    [Space()]
    public float followSpeed = 10.0f;

	[Space()]
	public float wallFadeOutTime = 0.1f;
	public float wallFadeInTime = 0.25f;

	void Awake()
	{
		Instance = this;
	}

    void LateUpdate()
    {
		Vector3 targetPos = Vector3.zero;

		if (players.Count > 0)
		{
			foreach (Transform target in players)
			{
				targetPos += target.position;
			}
		}

		targetPos /= players.Count;

		//Clamp to tile bounds
		//Only clamp if max is more than min
		if (max.x - paddingRight > min.x + paddingLeft)
			targetPos.x = Mathf.Clamp(targetPos.x, min.x + paddingLeft, max.x - paddingRight);
		else //Else centre
			targetPos.x = (min.x + max.x) / 2;

		if (max.z - paddingTop > min.z + paddingBottom)
			targetPos.z = Mathf.Clamp(targetPos.z, min.z + paddingBottom, max.z - paddingTop);
		else
			targetPos.z = (min.z + max.z) / 2;

		//Move camera to correct offset
		targetPos += Vector3.up * height;
        targetPos += Vector3.forward * zOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

	public void UpdateCameraBounds(Bounds tileBounds)
	{
		min = tileBounds.min;
		max = tileBounds.max;
	}
}
