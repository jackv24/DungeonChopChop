using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmmissionScaler : MonoBehaviour
{
	public Vector3 referenceSize = Vector3.one;

    public float multiplier = 1.0f;

	private ParticleSystem[] systems;
	private float[] initialEmissions;

	void Awake()
	{
		systems = GetComponentsInChildren<ParticleSystem>(true);

		initialEmissions = new float[systems.Length];

		for(int i = 0; i < initialEmissions.Length; i++)
		{
			initialEmissions[i] = systems[i].emission.rateOverTimeMultiplier;
		}
	}

	void OnEnable()
	{
		StartCoroutine(DelayFrame());
	}

	IEnumerator DelayFrame()
	{
		yield return new WaitForEndOfFrame();

		for(int i = 0; i < systems.Length; i++)
		{
			var shape = systems[i].shape;
			float ratio = shape.scale.magnitude / referenceSize.magnitude;

			var emission = systems[i].emission;
            emission.rateOverTimeMultiplier = ratio * initialEmissions[i] * multiplier;
		}
	}
}
