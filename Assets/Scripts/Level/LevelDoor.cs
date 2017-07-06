using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
	public LevelTile targetTile;
	public LevelDoor targetDoor;

	public LevelTile parentTile;

	private bool setup = false;
	private bool entered = false;

	public void SetTarget(LevelDoor target)
	{
		targetDoor = target;
		targetTile = target.GetComponentInParent<LevelTile>();

		parentTile = GetComponentInParent<LevelTile>();

        if (!setup)
            Setup();
	}

    void Setup()
    {
        setup = true;

		//Setup box collider through script to ensure all are the same
        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.size = new Vector3(5, 1, 0.5f);
        col.center = new Vector3(0, 0, 0.25f);
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if(setup)
        {
			//If player walked into this door
            if(col.tag == "Player" && col.GetType() == typeof(CharacterController))
            {
				if (!entered)
				{
					//if this door was entered on the current tile, enable target tile
					targetDoor.entered = true;

					targetTile.SetCurrent(parentTile);
				}
				else
				{
					//If this door was entered on the target tile, disable previous tile
					entered = false;
				}
            }
        }
    }
}
