using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour {

	public GameObject player;
	public int heartNumber;

	private Health playerHealth;
	private Image image;

	// Use this for initialization
	void Start () {
		heartNumber = transform.GetSiblingIndex () + 1;
		image = transform.GetChild(1).GetComponent<Image> ();
		if (player) {
			playerHealth = player.GetComponent<Health> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (playerHealth) {
			if (heartNumber > playerHealth.health && playerHealth.health > heartNumber - 1) {
				image.fillAmount = (playerHealth.health % 1);
			}
			if (playerHealth.health > heartNumber) {
				image.fillAmount = 1;
			}
			float temp = heartNumber - playerHealth.health;
			if (temp > 1) {
				image.fillAmount = 0;
			}
			if (heartNumber == playerHealth.health) {
				image.fillAmount = 1;
			}
		}
	}
}
