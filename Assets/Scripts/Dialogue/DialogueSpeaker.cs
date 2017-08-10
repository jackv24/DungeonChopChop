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

		//if colliders were found, player is in range
		if(cols.Length > 0)
		{
			//if box isn't already showing, show box
			if(!currentBox)
			{
				//Position pooled dialogue box
				GameObject obj = ObjectPooler.GetPooledObject(dialogueBoxPrefab);
				obj.transform.position = transform.position + Vector3.up * textBoxHeight;

				currentBox = obj.GetComponent<DialogueBox>();

				//Set random line
				if(currentBox)
					currentBox.SetDialogue(lines[Random.Range(0, lines.Length)]);
			}
		}
		else if(currentBox)
		{
			//Close dialoue and set null
			currentBox.CloseDialogue();

			currentBox = null;
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, speakRange);
	}
}
