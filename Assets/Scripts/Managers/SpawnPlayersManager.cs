using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayersManager : MonoBehaviour 
{
	[Header("Prefabs")]
	public GameObject player1Prefab;
	public GameObject player2Prefab;

    [Space()]
	public Vector3 spawnPosition;

	private bool player1Spawned = false;
	private int counter = 0;

    private CameraFollow cameraFollow;

	void Awake () 
	{
		cameraFollow = Camera.main.GetComponent<CameraFollow> ();

        //check if there is a player in the world
        //If there are no inputs, create one
        if (InputManager.Instance.playerInput.Count <= 0)
        {
            PlayerManager manager = GameObject.FindObjectOfType<PlayerManager>();
            if (manager)
            {
                manager.SinglePlayer();
            }
        }

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
        GameManager.Instance.players.Add(player.GetComponentInChildren<PlayerInformation>());
		player.GetComponent<PlayerInformation> ().playerIndex = counter;
		counter++;
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
