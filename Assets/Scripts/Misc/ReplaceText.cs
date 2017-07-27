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
	}

	void Start()
	{
		SetFallback();
	}

	public void Replace(string replaceText)
	{
		if (textString == "")
			textString = text.text;

		if (text)
			text.text = string.Format(textString, replaceText);
	}

	public void SetFallback()
	{
		if (textString == "")
			textString = text.text;

		if (text)
			text.text = string.Format(textString, fallBack);
	}
}
