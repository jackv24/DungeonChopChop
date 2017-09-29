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
        if (textElement != null)
		    textElement.text = text;
	}

	public void CloseDialogue()
	{
		// If there is a close anim, play that, else just disable
		if (closeAnim && anim)
			StartCoroutine(CloseAnimation());
		else
			gameObject.SetActive(false);
	}

	IEnumerator CloseAnimation()
	{
		// Play close anim
		anim.Play(closeAnim.name);

		// Wait until anim is finished
		yield return new WaitForSeconds(closeAnim.length);

		// Disable for pooling
		gameObject.SetActive(false);
	}
}
