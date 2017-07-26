using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
	[Tooltip("How far the player must walk out after entering this door.")]
	public float exitDistance = 1.0f;

	[Header("Set by Generator")]
	public LevelTile targetTile;
	public LevelDoor targetDoor;

	public LevelTile parentTile;

	private bool entered = false;

	public void SetTarget(LevelDoor target)
	{
		targetDoor = target;
		targetTile = target.GetComponentInParent<LevelTile>();

		parentTile = GetComponentInParent<LevelTile>();
	}

    void OnTriggerEnter(Collider col)
    {
		//If player walked into this door
        if(col.tag == "Player" && col.GetType() == typeof(CharacterController))
        {
			if (!entered)
			{
				//if this door was entered on the current tile, enable target tile
				targetDoor.entered = true;

				targetTile.SetCurrent(parentTile);

				StartCoroutine(WalkOut(col.gameObject));
			}
			else
			{
				//If this door was entered on the target tile, disable previous tile
				entered = false;
			}
        }
    }

	IEnumerator WalkOut(GameObject player)
	{
		PlayerInformation playerInfo = player.GetComponent<PlayerInformation>();
		PlayerMove playerMove = player.GetComponent<PlayerMove>();

		if(playerMove && playerInfo)
		{
			//Revoke player control
			playerMove.enabled = false;

			//Get move speed and direction
			float moveSpeed = playerInfo.moveSpeed;
			Vector3 direction = -transform.forward;

			//Calculate time required to move target distance
			float moveTime = exitDistance / moveSpeed;

			//Loop for that amount of time
			float elapsedTime = 0;
			while(elapsedTime <= moveTime)
			{
				//Move player
				player.transform.position += direction * moveSpeed * Time.deltaTime;

				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;
			}

			//Return player control
			playerMove.enabled = true;
		}
	}
}
