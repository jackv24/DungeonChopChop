using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour {

	public int coinAmount;

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player1" || col.tag == "Player2") {
            ItemsManager.Instance.Coins += coinAmount;
			gameObject.SetActive (false);
		}
	}

}
