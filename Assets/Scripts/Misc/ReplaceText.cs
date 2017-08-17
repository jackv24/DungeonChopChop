using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceText : MonoBehaviour
{
	public float changeTime = 0.5f;

	public string[] textOptions;

	public string fallBack = "Done!";

	private Text text;
	private string textString = "";

	private bool running = true;

	void Awake()
	{
		text = GetComponent<Text>();
	}

	void Start()
	{
		textString = text.text;

		StartCoroutine(ReplaceTick());
	}

	void OnEnable()
	{
		if (!running)
		{
			running = true;

			StartCoroutine(ReplaceTick());
		}
	}

	IEnumerator ReplaceTick()
	{
		if (textOptions.Length > 0)
		{
			int index = 0;

			while (running)
			{
				if (index >= textOptions.Length)
					index = 0;

				text.text = string.Format(textString, textOptions[index]);

				index++;

				yield return new WaitForSeconds(changeTime);
			}
		}

		text.text = string.Format(textString, fallBack);
	}

	public void SetFallback()
	{
		running = false;

		text.text = string.Format(textString, fallBack);
	}
}
