using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LookAtPlayer : MonoBehaviour
{
	public float range = 8.0f;
	public float rotateSpeed = 3.0f;
	public float angle = 135.0f;

	public Vector3 rotationOffset = new Vector3(0, 0, -90);

	private Quaternion currentRotation;
	private Quaternion targetRotation;

	private Quaternion initialRotation;
	private Vector3 initialForward;

	private Transform[] targets;

	void Start()
	{
		PlayerInformation[] infos = FindObjectsOfType<PlayerInformation>();

		targets = new Transform[infos.Length];

		for(int i = 0; i < targets.Length; i++)
		{
			targets[i] = infos[i].transform;
		}

		initialRotation = transform.rotation;
		initialForward = transform.forward;
	}

	void LateUpdate()
	{
		if (targets.Length > 0)
		{
			float closestDistance = float.MaxValue;
			Transform closest = null;

			foreach (Transform target in targets)
			{
				float distance = Vector3.Distance(transform.position, target.position);

				if (distance < closestDistance)
				{
					Vector3 playerDirection = target.position - transform.position;
					playerDirection.y = 0;

					if (Mathf.Abs(Vector3.Angle(initialForward, playerDirection)) <= angle / 2)
					{
						closestDistance = distance;
						closest = target;
					}
				}
			}

			if (closest && closestDistance < range)
			{
				Vector3 playerPos = closest.position;
				Vector3 npcPos = transform.position;

				Vector3 delta = playerPos - npcPos;
				delta.y = 0;

				targetRotation = Quaternion.LookRotation(delta);
				targetRotation.eulerAngles += rotationOffset;
			}
			else
				targetRotation = initialRotation;
		}
		else
			targetRotation = initialRotation;

		currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotateSpeed * Time.deltaTime);

		transform.rotation = currentRotation;
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Vector3 from = Quaternion.AngleAxis(-(angle / 2), Vector3.up) * (Application.isPlaying ? initialForward : transform.forward);

		Handles.color = new Color(0, 1, 1, 1);
		Handles.DrawWireArc(transform.position, Vector3.up, from, angle, range);
		Handles.color = new Color(0, 1, 1, 0.1f);
		Handles.DrawSolidArc(transform.position, Vector3.up, from, angle, range);
	}
#endif
}
