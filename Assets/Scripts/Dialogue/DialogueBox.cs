using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
	public AnimationClip closeAnim;

	private Animator anim;

	private Text textElement;

	void Awake()
	{
		anim = GetComponent<Animator>();

		textElement = GetComponentInChildren<Text>();
	}

	public void SetDialogue(string text)
	{
		textElement.text = text;
	}

	public void CloseDialogue()
	{
		if (closeAnim && anim)
			StartCoroutine(CloseAnimation());
		else
			gameObject.SetActive(false);
	}

	IEnumerator CloseAnimation()
	{
		anim.Play(closeAnim.name);

		yield return new WaitForSeconds(closeAnim.length);

		gameObject.SetActive(false);
	}
}
