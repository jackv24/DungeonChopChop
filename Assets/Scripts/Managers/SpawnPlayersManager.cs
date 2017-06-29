using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayersManager : MonoBehaviour 
{

	public GameObject playerPrefab;
	public Vector3 spawnPosition;

	void Start () 
	{
		int temp = 0;
		//loops through each player input and instantiates a player prefab
		foreach (PlayerInputs user in InputManager.Instance.playerInput) 
		{
			if (user != null) 
			{
				GameObject player = (GameObject)Instantiate (playerPrefab, spawnPosition, Quaternion.Euler (0, 0, 0));
				player.GetComponent<PlayerInformation> ().playerIndex = temp;
				temp++;
			}
		}
	}
}
