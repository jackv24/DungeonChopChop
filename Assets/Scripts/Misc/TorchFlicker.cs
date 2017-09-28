using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
	public float changeDelay = 0.1f;

	public float variance = 0.25f;
	private float max;
	private float min;

	private Light light;

	private Coroutine flickerRoutine;
	private LevelTile parentTile;

	void Awake()
	{
		light = GetComponent<Light>();
	}

	void Start()
	{
		parentTile = GetComponentInParent<LevelTile>();

		if(parentTile)
		{
			parentTile.OnTileEnter += SetActive;
			parentTile.OnTileExit += SetInactive;

			gameObject.SetActive(false);
		}


		min = light.intensity - variance;
		max = light.intensity + variance;
	}

	void OnEnable()
	{
		flickerRoutine = StartCoroutine(Flicker());
	}

	void OnDisable()
	{
		StopCoroutine(flickerRoutine);
	}

	void OnDestroy()
	{
		if (parentTile)
		{
			parentTile.OnTileEnter -= SetActive;
			parentTile.OnTileExit -= SetInactive;
		}
	}

	void SetActive()
	{
		gameObject.SetActive(true);
	}
	void SetInactive()
	{
		gameObject.SetActive(false);
	}

	IEnumerator Flicker()
	{
		float elapsed = 0;
		float targetIntensity;

		while (true)
		{
			yield return new WaitForSeconds(changeDelay);

			targetIntensity = Random.Range(min, max);

			while (elapsed < changeDelay)
			{
				light.intensity = Mathf.Lerp(light.intensity, targetIntensity, elapsed / changeDelay);

				yield return new WaitForEndOfFrame();
				elapsed += Time.deltaTime;
			}

			elapsed = 0;
		}
	}
}
