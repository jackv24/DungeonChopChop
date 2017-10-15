using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
	[Tooltip("How far the player must walk out after entering this door.")]
	public float exitDistance = 2.0f;

	[Header("Exit Decals")]
	public GameObject grassDecalPrefab;
	public GameObject desertDecalPrefab;
	public GameObject fireDecalPrefab;
	public GameObject iceDecalPrefab;
	public GameObject forestDecalPrefab;
	public GameObject dungeonDecalPrefab;

	private GameObject oldDecalGraphic = null;

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
			if (!entered)
			{
				//if this door was entered on the current tile, enable target tile
				targetDoor.entered = true;

				//Walk out into next tile
				StartCoroutine(WalkOut(col.gameObject));

				//Create event to restore player movement on tile enter
				LevelTile.NormalEvent enterEvent = null;
				enterEvent = delegate
				{
					PlayerInformation[] players = FindObjectsOfType<PlayerInformation>();

					foreach (PlayerInformation p in players)
					{
						Health health = p.GetComponent<Health>();

						//Only move player to door exit if they are alive
						if (!health || health.health > 0)
							p.transform.position = transform.position + (-transform.forward) * exitDistance;

						//Re-enable all player scripts
						PlayerMove move = p.GetComponent<PlayerMove>();
						PlayerAttack attack = p.GetComponent<PlayerAttack>();
						move.enabled = true;
						attack.enabled = true;
					}

					//Remove self once complete
					targetTile.OnTileEnter -= enterEvent;
				};
				//Subscribe event
				targetTile.OnTileEnter += enterEvent;

				//Transition to new tile
				targetTile.SetCurrent(parentTile);
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
		PlayerInformation[] players = FindObjectsOfType<PlayerInformation>();

		//Disable player scripts
		foreach (PlayerInformation p in players)
		{
			PlayerMove move = p.GetComponent<PlayerMove>();
			PlayerAttack attack = p.GetComponent<PlayerAttack>();
			move.enabled = false;
			attack.enabled = false;
		}

		Vector3 direction = -transform.forward;

		PlayerInformation playerInfo = player.GetComponent<PlayerInformation>();

		if (playerInfo)
		{
			//Get move speed and direction
			float moveSpeed = playerInfo.maxMoveSpeed;

			Vector3 targetPos = transform.position + direction * exitDistance;

			//Calculate time required to move target distance
			float moveTime = Vector3.Distance(targetPos, transform.position) / moveSpeed;

			//Loop for that amount of time
			float elapsedTime = 0;
			while(elapsedTime <= moveTime)
			{
				//Move player
				player.transform.position += direction * moveSpeed * Time.deltaTime;

				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;
			}
		}
	}

	public void ReplaceGraphic(LevelTile.Biomes biome)
	{
		GameObject newGraphic = null;

		switch (biome)
		{
			case LevelTile.Biomes.Grass:
				newGraphic = grassDecalPrefab;
				break;
			case LevelTile.Biomes.Desert:
				newGraphic = desertDecalPrefab;
				break;
			case LevelTile.Biomes.Fire:
				newGraphic = fireDecalPrefab;
				break;
			case LevelTile.Biomes.Ice:
				newGraphic = iceDecalPrefab;
				break;
			case LevelTile.Biomes.Forest:
				newGraphic = forestDecalPrefab;
				break;
			case LevelTile.Biomes.Dungeon:
				newGraphic = dungeonDecalPrefab;
				break;
		}

		if (newGraphic)
		{
			GameObject obj = Instantiate(newGraphic, transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;

			if (oldDecalGraphic)
				DestroyImmediate(oldDecalGraphic);

			oldDecalGraphic = obj;
		}
	}
}
