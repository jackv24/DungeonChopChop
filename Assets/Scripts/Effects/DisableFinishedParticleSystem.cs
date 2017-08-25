using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFinishedParticleSystem : MonoBehaviour
{
	private ParticleSystem system;

	[Tooltip("How long after spawning until checking if disabled starts.")]
	public float firstCheckDelay = 0.0f;
	private float firstCheckTime;

	private void Awake()
	{
		system = GetComponent<ParticleSystem>();
	}

	private void Start()
	{
		firstCheckTime = Time.time + firstCheckDelay;
	}

	//Only needs to check every now and then
	private void FixedUpdate()
	{
		if(system)
		{
			if (system.isStopped && Time.time >= firstCheckTime)
				gameObject.SetActive(false);
		}
	}
}
