using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHearts : MonoBehaviour {

	private GameObject player;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		if (player) {
			foreach (Transform child in transform) {
				child.GetComponent<Heart> ().player = player;
			}
		}
	}

}
