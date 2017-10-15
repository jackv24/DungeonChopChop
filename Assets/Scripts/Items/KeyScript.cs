using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : PickupableItems
{
	public enum Type { Normal, Dungeon }
	public Type type = Type.Normal;

	public InventoryItem keyItem;

    private int counter = 0;

    void OnEnable()
    {
        //push the item out a bit
        Vector3 direction = new Vector3(Random.insideUnitSphere.x * force, 1, Random.insideUnitSphere.z * force);
        GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);

        counter = 0;
        canPickup = false;
        DidEnable();
        Physics.IgnoreLayerCollision(15, 14, true);
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(.1f);
        DoSpawnParticle(transform.position);
    }

    // Update is called once per frame
    void FixedUpdate () 
    {
        counter++;
        if (counter > pickupDelay * 60)
        {
            Physics.IgnoreLayerCollision(15, 14, false);
            canPickup = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (canPickup)
        {
            //checks if the player collides with the item
            if (col.tag == "Player1" || col.tag == "Player2")
            {
                Pickup(col.GetComponent<PlayerInformation>());
            }
        }
    }

	void Pickup(PlayerInformation playerInfo)
	{
		if (type == Type.Normal)
			ItemsManager.Instance.Keys += 1 * (int)playerInfo.GetCharmFloat("keyMultiplier");
		else
			ItemsManager.Instance.DungeonKeys += 1;

        DoPickUpParticle();
		gameObject.SetActive(false);
	}

    void DoPickUpParticle()
    {
        //do particles
        spawnEffects.EffectOnDeath(particles, transform.position);

        SoundManager.PlaySound(pickUpSound, transform.position);
    }
}
