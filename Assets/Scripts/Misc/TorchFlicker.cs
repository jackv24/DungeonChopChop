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

	void Awake()
	{
		light = GetComponent<Light>();
	}

	void Start()
	{
		LevelTile parentTile = GetComponentInParent<LevelTile>();

		if(parentTile)
		{
			parentTile.OnTileEnter += delegate { gameObject.SetActive(true); };
			parentTile.OnTileExit += delegate { gameObject.SetActive(false); };

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
