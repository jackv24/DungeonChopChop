using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayersManager : MonoBehaviour 
{
	[Header("Prefabs")]
	public GameObject player1Prefab;
	public GameObject player2Prefab;
	[Header("UI Parents")]
	public GameObject player1UI;
	public GameObject player2UI;
	public Vector3 spawnPosition;

	private bool player1Spawned = false;
	private int counter = 0;

	void Awake () 
	{
		player2UI.SetActive (false);
		CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow> ();
		//loops through each player input and instantiates a player prefab
		if (InputManager.Instance) 
		{
			foreach (PlayerInputs user in InputManager.Instance.playerInput) 
			{
				if (user != null) 
				{
					if (!player1Spawned) 
					{
						player1Spawned = true;
						SpawnPlayer (cameraFollow, player1Prefab);
					} 
					else {
						SpawnPlayer (cameraFollow, player2Prefab);
					}
				}
			}
		}
	}

	//Instantiates the player prefab
	void SpawnPlayer(CameraFollow cameraFollow, GameObject obj)
	{
		GameObject player = (GameObject)Instantiate (obj, new Vector3(spawnPosition.x + (counter * 10), spawnPosition.y, spawnPosition.z), Quaternion.Euler (0, 0, 0));

		player.GetComponent<PlayerInformation> ().playerIndex = counter;
		counter++;
		SetUI ();
	}

	void SetUI()
	{
		if (counter == 1) {
			player1UI.SetActive (true);
		} else if (counter == 2) {
			player2UI.SetActive (true);
		}
	}

}
