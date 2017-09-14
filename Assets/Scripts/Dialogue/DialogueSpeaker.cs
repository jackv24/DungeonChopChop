﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
	public delegate void PlayerGet(PlayerInformation playerInfo, bool value);
	public event PlayerGet OnGetPlayer;

	[Multiline]
	public string[] lines = { "Default text" };
	private int lastIndex = -1;

	[Space()]
	public Vector3 speakOffset;
	public Vector3 speakArea;
	public LayerMask playerLayer;
	public float textBoxHeight = 3.0f;
	public float textBoxDepth = 0.0f;

	[Space()]
	public GameObject dialogueBoxPrefab;

	private DialogueBox currentBox;
	private PlayerInformation playerInfo;

	//Does not need to happen every frame
	void FixedUpdate()
	{
		Collider[] cols = Physics.OverlapBox(transform.position + speakOffset, speakArea / 2, transform.rotation, playerLayer);

		//if colliders were found, player is in range
		if (cols.Length > 0)
		{
			//if box isn't already showing, show box
			if (!currentBox)
			{
				Open(cols[0].gameObject);
			}
		}
		else if (currentBox)
			Close();
	}

	void Open(GameObject player)
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

			if (OnGetPlayer != null)
			{
				playerInfo = player.GetComponent<PlayerInformation>();

				OnGetPlayer(playerInfo, true);
			}
		}
	}

	public void Close(bool disable = false)
	{
		//Close dialouge and set null
		currentBox.CloseDialogue();

		currentBox = null;

		if (OnGetPlayer != null)
			OnGetPlayer(playerInfo, false);

		if (disable)
			enabled = false;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(speakOffset, speakArea);
	}
}
