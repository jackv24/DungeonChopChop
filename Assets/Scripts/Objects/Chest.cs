using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool opened = false;
    public bool requireKeys = false;

	public Helper.ProbabilityItem[] possibleItems;
	public BaseItem containingItem;

	public float releaseItemDelay = 1.5f;
	public float releaseItemForce = 10.0f;

    private Animator animator;

	void Start ()
	{
        animator = GetComponentInChildren<Animator>();

		containingItem = Helper.GetRandomItemByProbability(possibleItems);
	}

    void OnCollisionEnter(Collision col)
    {
        if (!opened)
        {
            if (col.collider.gameObject.layer == 14)
            {
				if (ItemsManager.Instance)
				{
                    if (requireKeys)
                    {
                        if (ItemsManager.Instance.Keys > 0)
                        {
                            Open();
                            ItemsManager.Instance.Keys -= 1;
                        }
                    }
                    else
                    {
                        Open();
                    }
				}
				else //If there is no items manager, just open the chest anyway (for testing)
					Open();
            }
        }
    }

	void Open()
	{
        //opens chest and plays animation
		animator.SetTrigger("Open");
		opened = true;

		if(containingItem)
			StartCoroutine(ReleaseItems());

		//Remove icon from map once opened
		MapTracker icon = GetComponent<MapTracker>();
		if (icon)
			icon.Remove();
	}

	IEnumerator ReleaseItems()
	{
		yield return new WaitForSeconds(releaseItemDelay);

		//Spawn object
		GameObject obj = ObjectPooler.GetPooledObject(LevelVars.Instance.droppedItemPrefab);
		obj.transform.position = transform.position + Vector3.up;

		//Set containing item
		ItemPickup pickup = obj.GetComponent<ItemPickup>();
		if (pickup)
			pickup.representingItem = containingItem;

		//Throw out of chest
		Rigidbody body = obj.GetComponent<Rigidbody>();
		body.AddForce(Vector3.up * releaseItemForce, ForceMode.Impulse);
	}
}
