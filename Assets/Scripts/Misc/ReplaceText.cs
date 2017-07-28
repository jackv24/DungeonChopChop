using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceText : MonoBehaviour
{
	public string fallBack = "Done!";

	private Text text;
	private string textString = "";

	void Awake()
	{
		text = GetComponent<Text>();

		Replace("");
	}

	public void Replace(string replaceText)
	{
		if (text)
		{
			if (textString == "")
				textString = text.text;

			text.text = string.Format(textString, replaceText);
		}
	}

	public void SetFallback()
	{
		if (text)
		{
			if (textString == "")
				textString = text.text;

			text.text = string.Format(textString, fallBack);
		}
	}
}
