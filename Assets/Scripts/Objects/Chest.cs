using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ChestType
{
    [Tooltip("Spawn Items such as gems, keys etc")]
    Iron,
    [Tooltip("Spawns Charms and Weapons")]
    Gold,
    [Tooltip("Spawns Dungeon Item")]
    Dungeon
}

public class Chest : MonoBehaviour
{
	[HideInInspector]
    public bool opened = false;
    public bool requireKeys = false;

	public KeyScript.Type keyType = KeyScript.Type.Normal;
    public ChestType chestType;

	public Helper.ProbabilityItem[] possibleItems;
    public Helper.ProbabilityGameObject[] possibleObjects;
    public int minAmountOfObjects = 2;
    public int maxAmountOfObjects = 5;


    [HideInInspector]
	public BaseItem containingItem;
    [HideInInspector]
    public List<GameObject> containingObjects = new List<GameObject>(0);

	private bool randomise = true;

	public float releaseItemDelay = 1.5f;
	public float releaseItemForce = 10.0f;

    private Animator animator;

	void Start ()
	{
        animator = GetComponentInChildren<Animator>();

        //populate the chest with gold items
        if (chestType == ChestType.Gold)
        {
            if (randomise)
                containingItem = Helper.GetRandomItemByProbability(possibleItems);
        }
        else if (chestType == ChestType.Iron)
        {
            if (possibleObjects.Length > 0)
            {
                int random = Random.Range(minAmountOfObjects, maxAmountOfObjects);
                for (int i = 0; i < random; i++)
                {
                    GameObject obj = Helper.GetRandomGameObjectByProbability(possibleObjects);
                    Debug.Log(obj.name);
                    containingObjects.Add(obj);
                }
            }
        }
	}

	public void SetItem(BaseItem item)
	{
		randomise = false;
		containingItem = item;
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
                        if ((keyType == KeyScript.Type.Normal && ItemsManager.Instance.Keys > 0) || (keyType == KeyScript.Type.Dungeon && ItemsManager.Instance.DungeonKeys > 0))
                        {
                            Open();

							if(keyType == KeyScript.Type.Normal)
								ItemsManager.Instance.Keys -= 1;
							else
								ItemsManager.Instance.DungeonKeys -= 1;
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

        if (containingItem || containingObjects.Count > 0)
        {
            if (chestType == ChestType.Gold)
                StartCoroutine(ReleaseItems());
            else if (chestType == ChestType.Iron)
                StartCoroutine(ReleaseObjects());
        }
			

		//Remove icon from map once opened
		MapTracker icon = GetComponent<MapTracker>();
		if (icon)
			icon.Remove();

		EventSender events = GetComponentInParent<EventSender>();
        if (events)
            events.SendDisabledEvent();
	}

    IEnumerator ReleaseObjects()
    {
        yield return new WaitForSeconds(releaseItemDelay);

        foreach (GameObject o in containingObjects)
        {
            GameObject obj = ObjectPooler.GetPooledObject(o);
            //throw out of chest
            obj.transform.position = transform.position + Vector3.up;

            Vector3 direction = new Vector3(Random.insideUnitSphere.x, 1, Random.insideUnitSphere.z);
            GetComponent<Rigidbody>().AddForce(direction * releaseItemForce, ForceMode.Impulse);
        }
    }

	IEnumerator ReleaseItems()
	{
		yield return new WaitForSeconds(releaseItemDelay);

		//Spawn object
		GameObject obj = null;

		//Set containing item
		if (containingItem is Charm)
		{
			obj = ObjectPooler.GetPooledObject(LevelVars.Instance.droppedCharmPrefab);

			CharmPickup pickup = obj.GetComponentInChildren<CharmPickup>();
			if (pickup)
				pickup.representingCharm = (Charm)containingItem;
		}
		else if(containingItem is InventoryItem)
		{
			InventoryItem item = (InventoryItem)containingItem;

			if(item.itemPrefab)
			{
				obj = ObjectPooler.GetPooledObject(item.itemPrefab);
			}
		}

		if (obj)
		{
			obj.transform.position = transform.position + Vector3.up;

			//Throw out of chest
			Rigidbody body = obj.GetComponent<Rigidbody>();
			if(body)
				body.AddForce(Vector3.up * releaseItemForce, ForceMode.Impulse);
		}
	}
}
