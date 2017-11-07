using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour {

	public GameObject player;
	public int heartNumber;

	private Health playerHealth;
	private Image image;
	public Image[] childImages;

	private bool containerActive = true;

	// Use this for initialization
	void Start () {
		heartNumber = transform.GetSiblingIndex () + 1;
		image = transform.GetChild(1).GetComponent<Image> ();

		StartCoroutine (wait ());

		childImages = transform.GetComponentsInChildren<Image> ();
	}

	IEnumerator wait()
	{
		yield return new WaitForSeconds (.1f);
		if (player) {
			playerHealth = player.GetComponent<Health> ();
		}
        playerHealth.OnHealthChange += HealthChange;
        HealthChange();
	}

    void HealthChange()
    {
        if (playerHealth) {
            //checks to see if the heart number is greater then the player health ... eg heart number (7) is greater then health (7.4)
            if (heartNumber > playerHealth.health && playerHealth.health > heartNumber - 1) {
                //it then sets the fill amount of the image depending on the decimal point of health
                image.fillAmount = (playerHealth.health % 1);
            }
            //safety checker, if health is greater then heart number it needs to be filled
            if (playerHealth.health > heartNumber) {
                //Debug.Log (heartNumber + "player health is greater then this heart number");
                image.fillAmount = 1;
            }
            //safety checker, checks if health is less then heart number, if so make sure the heart is empty
            float temp = heartNumber - playerHealth.health;
            if (temp > 1) {
                image.fillAmount = 0;
            }
            //safety checker, for the last heart, if the health is full, fill the heart
            if (heartNumber == playerHealth.health) {
                image.fillAmount = 1;
            }
            //check if number is an int
            if (playerHealth.health % 1 == 0)
            {
                if (heartNumber > playerHealth.health)
                {
                    image.fillAmount = 0;
                }
            }
            //hide heart container
            if (heartNumber > playerHealth.maxHealth) 
            {
                if (containerActive) 
                {
                    foreach (Image child in childImages) 
                    {
                        child.gameObject.SetActive (false);
                    }
                    containerActive = false;
                }
            } 
            else 
            {
                if (!containerActive) 
                {
                    foreach (Image child in childImages) 
                    {
                        child.gameObject.SetActive (true);
                    }
                    containerActive = true;
                }
            }
        }
    }
}
