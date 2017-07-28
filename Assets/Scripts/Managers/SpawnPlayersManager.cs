using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayersManager : MonoBehaviour 
{

	public GameObject playerPrefab;
	public Vector3 spawnPosition;

	void Start () 
	{
		CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow> ();
		int temp = 0;
		//loops through each player input and instantiates a player prefab
		if (InputManager.Instance) 
		{
			foreach (PlayerInputs user in InputManager.Instance.playerInput) 
			{
				if (user != null) 
				{
					GameObject player = (GameObject)Instantiate (playerPrefab, new Vector3(spawnPosition.x + (temp * 10), spawnPosition.y, spawnPosition.z), Quaternion.Euler (0, 0, 0));
					if (!cameraFollow.player)
						cameraFollow.player = player.transform;
					
					player.GetComponent<PlayerInformation> ().playerIndex = temp;
					temp++;
				}
			}
		}
	}
}
