using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomText : MonoBehaviour
{
	public string[] strings;

	private Text text;

	void Awake()
	{
		text = GetComponent<Text>();
	}

	void Start()
	{
		Randomise();
	}

	public void Randomise()
	{
		if (text && strings.Length > 0)
		{
			text.text = strings[Random.Range(0, strings.Length)];
		}
	}
}
