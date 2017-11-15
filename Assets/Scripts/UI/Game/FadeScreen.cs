using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
	public static FadeScreen Instance;

	public float fadeInTime = 0.25f;
	public float fadeOutTime = 0.1f;
	public float pauseTime = 0.1f;

    public bool faded = false;

	private Image image;

	private Coroutine lastRoutine;

	private void Awake()
	{
		Instance = this;

		image = GetComponentInChildren<Image>();

		if (image)
			image.gameObject.SetActive(false);
	}

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	public void FadeInOut()
	{
		if (image)
		{
			if(lastRoutine != null)
				StopCoroutine(lastRoutine);

			lastRoutine = StartCoroutine(Fade());
		}
	}

	IEnumerator Fade()
	{
        faded = true;

		image.gameObject.SetActive(true);

		Color color = image.color;
		float initialAlpha = color.a;

		//Fade image in
		float elapsed = 0;
		while(elapsed < fadeOutTime)
		{
			color.a = Mathf.Lerp(initialAlpha, 1, elapsed / fadeOutTime);
			image.color = color;

			yield return new WaitForEndOfFrame();
			elapsed += Time.deltaTime;
		}

		//make sure it is completely opaque
		color.a = 1.0f;
		image.color = color;

        faded = false;

		//Wait for some time
		yield return new WaitForSeconds(pauseTime);

		//Fade image out
		elapsed = 0;
		while (elapsed < fadeInTime)
		{
			color.a = Mathf.Lerp(1, 0, elapsed / fadeInTime);
			image.color = color;

			yield return new WaitForEndOfFrame();
			elapsed += Time.deltaTime;
		}

		//Make sure it is completely transparent
		color.a = 0;
		image.color = color;

		image.gameObject.SetActive(false);
	}
}
