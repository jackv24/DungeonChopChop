using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharmImage : MonoBehaviour {

	private PlayerCharm playerCharm;
	private PlayerInformation playerInfo;
	private Image charmImg;

	// Use this for initialization
	void Start () {
		StartCoroutine (wait ());
		charmImg = GetComponent<Image> ();
	}

	IEnumerator wait()
	{
		yield return new WaitForSeconds (.1f);
		GameObject s = GameObject.FindGameObjectWithTag ("Player");
		if (s) {
			playerCharm = s.GetComponent<PlayerCharm> ();
			playerInfo = s.GetComponent<PlayerInformation> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (playerCharm) {
			if (playerInfo.currentCharm == Charms.ArmorBugCharm) {
				charmImg.sprite = playerCharm.armorCharmImg;
			} else if (playerInfo.currentCharm == Charms.DashCharm) {
				charmImg.sprite = playerCharm.dashCharmImg;
			}
			else if (playerInfo.currentCharm == Charms.DeathTouchCharm) {
				charmImg.sprite = playerCharm.deathTouchCharmImg;
			}
			else if (playerInfo.currentCharm == Charms.ReacherCharm) {
				charmImg.sprite = playerCharm.reacherCharmImg;
			}
			else if (playerInfo.currentCharm == Charms.StrengthCharm) {
				charmImg.sprite = playerCharm.strengthCharmImg;
			}
			else if (playerInfo.currentCharm == Charms.ThiefsCharm) {
				charmImg.sprite = playerCharm.thiefCharmImg;
			}
		}
	}
}
