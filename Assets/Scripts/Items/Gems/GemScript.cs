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

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player1" || col.tag == "Player2") {
            ItemsManager.Instance.Coins += coinAmount;
			gameObject.SetActive (false);
		}
	}

}
