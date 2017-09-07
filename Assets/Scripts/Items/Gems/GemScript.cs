using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour {

	public int coinAmount;

	void OnTriggerEnter(Collider col)
	{
        //checks if the player collides with the item
		if (col.tag == "Player1" || col.tag == "Player2") {
            ItemsManager.Instance.Coins += coinAmount * (int)col.GetComponent<PlayerInformation>().GetCharmFloat("coinMultiplier");
			gameObject.SetActive (false);
		}
	}
}
