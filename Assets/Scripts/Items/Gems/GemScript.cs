using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour {

	public int coinAmount;
	private ItemsManager items;

	// Use this for initialization
	void Start () {
		GameObject s = GameObject.FindGameObjectWithTag ("ItemsManager");
		if (s)
			items = s.GetComponent<ItemsManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag == "Player1" || col.collider.tag == "Player2") {
			items.Coins += coinAmount;
			gameObject.SetActive (false);
		}
	}

}
