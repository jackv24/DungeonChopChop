using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
	[Tooltip("How far the player must walk out after entering this door.")]
	public float exitDistance = 2.0f;
	public float secondaryExitDistance = 1.0f;

	[Header("Set by Generator")]
	public LevelTile targetTile;
	public LevelDoor targetDoor;

	public LevelTile parentTile;

	private bool entered = false;

	private MeshRenderer rend;

	private void Start()
	{
		rend = GetComponentInChildren<MeshRenderer>();

		if (rend)
			rend.enabled = false;
	}

	public void ShowOnMap()
	{
		if(rend)
			rend.enabled = true;
	}

	public void SetTarget(LevelDoor target)
	{
		targetDoor = target;
		targetTile = target.GetComponentInParent<LevelTile>();

		parentTile = GetComponentInParent<LevelTile>();
	}

    void OnTriggerEnter(Collider col)
    {
		//If player walked into this door
		if(col.gameObject.layer == 14 && col.GetType() == typeof(CharacterController))
        {
			//Only player 1 can enter doors
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
		PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();

		Vector3 direction = -transform.forward;

		if (playerAttack)
			playerAttack.enabled = false;

		if (playerMove && playerInfo)
		{
			//Revoke player control
			playerMove.enabled = false;

			//Get move speed and direction
			float moveSpeed = playerInfo.maxMoveSpeed;

			Vector3 targetPos = transform.position + direction * exitDistance;

			//Calculate time required to move target distance
			float moveTime = Vector3.Distance(targetPos, player.transform.position) / moveSpeed;

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

		if (playerAttack)
			playerAttack.enabled = true;

		//Position other player outside door
		PlayerInformation[] players = FindObjectsOfType<PlayerInformation>();
		foreach(PlayerInformation p in players)
		{
			if (p.gameObject != player)
				p.transform.position = transform.position + direction * secondaryExitDistance;
		}
	}
}
