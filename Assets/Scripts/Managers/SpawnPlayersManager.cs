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

    private CameraFollow cameraFollow;

	void Awake () 
	{
		player2UI.SetActive (false);
		cameraFollow = Camera.main.GetComponent<CameraFollow> ();
		
        DoSpawning();
	}

    void DoSpawning()
    {
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
        GameObject player = (GameObject)Instantiate (obj, transform.position, Quaternion.Euler (0, 0, 0));
        GameManager.Instance.players.Add(player.GetComponent<PlayerInformation>());
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

    public void ResetMono()
    {
        counter = 0;
        player1Spawned = false;
        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            Destroy(player.gameObject);
        }
        GameManager.Instance.players = null;
        DoSpawning();
    }
}
