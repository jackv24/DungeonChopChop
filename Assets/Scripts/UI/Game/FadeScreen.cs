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

	private Image image;

	private void Awake()
	{
		Instance = this;

		image = GetComponentInChildren<Image>();

		if (image)
			image.gameObject.SetActive(false);
	}

	public void FadeInOut()
	{
		if (image)
		{
			StopCoroutine("Fade");
			StartCoroutine(Fade());
		}
	}

	IEnumerator Fade()
	{
		image.gameObject.SetActive(true);

		Color color = image.color;

		//Fade image in
		float elapsed = 0;
		while(elapsed < fadeInTime)
		{
			color.a = Mathf.Lerp(0, 1, elapsed / fadeInTime);
			image.color = color;

			yield return new WaitForEndOfFrame();
			elapsed += Time.deltaTime;
		}

		//make sure it is completely opaque
		color.a = 1.0f;
		image.color = color;

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
