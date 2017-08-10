using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
	public string[] lines = { "Default text" };

	[Space()]
	public float speakRange = 2.0f;
	public LayerMask playerLayer;
	public float textBoxHeight = 3.0f;

	[Space()]
	public GameObject dialogueBoxPrefab;

	private DialogueBox currentBox;

	//Does not need to happen every frame
	void FixedUpdate()
	{
		Collider[] cols = Physics.OverlapSphere(transform.position, speakRange, playerLayer);

		if(cols.Length > 0)
		{
			if(!currentBox)
			{
				GameObject obj = ObjectPooler.GetPooledObject(dialogueBoxPrefab);
				obj.transform.position = transform.position + Vector3.up * textBoxHeight;

				currentBox = obj.GetComponent<DialogueBox>();

				if(currentBox)
				{
					currentBox.SetDialogue(lines[Random.Range(0, lines.Length)]);
				}
			}
		}
		else if(currentBox)
		{
			currentBox.CloseDialogue();

			currentBox = null;
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, speakRange);
	}
}
