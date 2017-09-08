﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
	[Multiline]
	public string[] lines = { "Default text" };
	private int lastIndex = -1;

	[Space()]
	public float speakRange = 2.0f;
	public LayerMask playerLayer;
	public float textBoxHeight = 3.0f;
	public float textBoxDepth = 0.0f;

	[Space()]
	public List<DialogueSpeaker> group = new List<DialogueSpeaker>();

	[Space()]
	public GameObject dialogueBoxPrefab;

	private DialogueBox currentBox;

	//Does not need to happen every frame
	void FixedUpdate()
	{
		Collider[] cols = Physics.OverlapSphere(transform.position, speakRange, playerLayer);

		//if colliders were found, player is in range
		if (cols.Length > 0)
		{
			bool show = true;

			if (group.Count > 1)
			{
				float closestDistance = float.MaxValue;
				DialogueSpeaker closestSpeaker = null;

				foreach(DialogueSpeaker speaker in group)
				{
					float distance = float.MaxValue;

					foreach(Collider col in cols)
					{
						float d = Vector3.Distance(speaker.transform.position, col.transform.position);

						if (d < distance)
							distance = d;
					}

					if(distance < closestDistance)
					{
						closestDistance = distance;
						closestSpeaker = speaker;
					}
				}

				if (closestSpeaker != this)
					show = false;
			}

			if (show)
			{
				//if box isn't already showing, show box
				if (!currentBox)
				{
					//Position pooled dialogue box
					GameObject obj = ObjectPooler.GetPooledObject(dialogueBoxPrefab);
					obj.transform.position = transform.position + new Vector3(0, textBoxHeight, textBoxDepth);

					currentBox = obj.GetComponent<DialogueBox>();

					//Set random line
					if (currentBox)
					{
						if (lines.Length > 0)
						{
							int index = lastIndex;

							if (lines.Length > 1)
							{
								while (index == lastIndex)
									index = Random.Range(0, lines.Length);
							}
							else
								index = 0;

							currentBox.SetDialogue(lines[index]);

							lastIndex = index;
						}
						else
							currentBox.SetDialogue("!NO LINES!");
					}
				}
			}
			else if (currentBox)
				Close();
		}
		else if (currentBox)
			Close();
	}

	void Close()
	{
		//Close dialoue and set null
		currentBox.CloseDialogue();

		currentBox = null;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, speakRange);
	}
}
