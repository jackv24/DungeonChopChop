using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
	public LevelTile targetTile;
	public LevelDoor targetDoor;

    private bool setup = false;
	private bool entered = false;

	public void SetTarget(LevelDoor target)
	{
		targetDoor = target;
		targetTile = target.GetComponentInParent<LevelTile>();

        if (!setup)
            Setup();
	}

    void Setup()
    {
        setup = true;

		//Setup box collider through script to ensure all are the same
        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.size = new Vector3(3, 1, 1);
        col.center = new Vector3(0, 0, -0.5f);
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
					targetTile.gameObject.SetActive(true);
					targetDoor.entered = true;
				}
				else
				{
					//If this door was entered on the target tile, disable previous tile
					targetTile.gameObject.SetActive(false);
					entered = false;
				}
            }
        }
    }
}
