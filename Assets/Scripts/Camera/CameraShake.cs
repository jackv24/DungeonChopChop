using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance;

	[Tooltip("Can be used to lower the amount of screenshake throughout.")]
	public float multiplier = 1.0f;

	void Awake()
	{
		Instance = this;
	}

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	/// <summary>
	/// Causes the screen to shake according to parameters.
	/// </summary>
	/// <param name="magnitude">The amount that the camera will be offset.</param>
	/// <param name="shakeAmount">How many "sub-shakes" within this camera shake.</param>
	/// <param name="duration">How long the screen shake lasts for.</param>
	public static void ShakeScreen(float magnitude, float shakeAmount, float duration)
	{
		if(Instance)
			Instance.StartCoroutine(Instance.Shake(magnitude * Instance.multiplier, duration / shakeAmount, duration));
	}

	IEnumerator Shake(float magnitude, float shakeStep, float duration)
	{
		float elapsedTime = 0;
		float elapsedStep = 0;

		Vector2 offset = Random.insideUnitCircle.normalized * magnitude;

		while(elapsedTime < duration)
		{
			transform.Translate(Vector3.Lerp(offset, Vector3.zero, elapsedTime / duration), Space.Self);

			yield return new WaitForEndOfFrame();
			elapsedTime += Time.unscaledDeltaTime;
			elapsedStep += Time.unscaledDeltaTime;

			if(elapsedStep >= shakeStep)
			{
				elapsedStep = 0;

				offset = Random.insideUnitCircle.normalized * magnitude;
			}
		}
    }
}
