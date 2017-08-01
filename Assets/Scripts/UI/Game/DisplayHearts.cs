using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHearts : MonoBehaviour {

	public string playerTag;
	private GameObject player;

	void Start()
	{
		//assign the hearts variable player .... important for 2 player
		player = GameObject.FindGameObjectWithTag (playerTag);
		if (player) {
			foreach (Transform child in transform) {
				child.GetComponent<Heart> ().player = player;
			}
		}
	}

}
