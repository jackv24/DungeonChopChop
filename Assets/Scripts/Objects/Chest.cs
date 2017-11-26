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
    public delegate void NormalEvent();
    public event NormalEvent OnChestOpen;

    [HideInInspector]
    public bool opened = false;
    public bool requireKeys = false;

	public bool randomise = true;

	public KeyScript.Type keyType = KeyScript.Type.Normal;
    public ChestType chestType;

    [Tooltip("Charms")]
	public Helper.ProbabilityItem[] possibleItems;
    [Tooltip("Gems, keys, orbs etc")]
    public Helper.ProbabilityGameObject[] possibleConsumables;
    [Tooltip("Weapons, armour etc")]
    public Helper.ProbabilityGameObject[] possibleObjects;
    public int minAmountOfObjects = 2;
    public int maxAmountOfObjects = 5;
    public LayerMask playerMask;

	public BaseItem containingItem;
	[HideInInspector]
	public GameObject containingObject;
	[HideInInspector]
	public List<GameObject> containingConsumables = new List<GameObject>(0);

	[Header("Item Values")]
    public GameObject animationBone;
    public float releaseItemDelay = 1.5f;
    public float releaseItemForce = 10.0f;

    [Header("Audio")]
    public SoundEffect openSound;

    [Header("Particles")]
    public GameObject releaseParticle;

    private Animator animator;

	void Start ()
	{
        animator = GetComponentInChildren<Animator>();

        //populate the chest with gold items
        if (chestType == ChestType.Gold)
        {
            if (randomise)
            {
                containingItem = Helper.GetRandomItemByProbability(possibleItems);
                containingObject = Helper.GetRandomGameObjectByProbability(possibleObjects);
            }
        }
        else if (chestType == ChestType.Iron)
        {
            if (possibleConsumables.Length > 0)
            {
                int random = Random.Range(minAmountOfObjects, maxAmountOfObjects);
                for (int i = 0; i < random; i++)
                {
                    GameObject obj = Helper.GetRandomGameObjectByProbability(possibleConsumables);
                    containingConsumables.Add(obj);
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

							if (keyType == KeyScript.Type.Normal) {
								ItemsManager.Instance.Keys -= 1;
								ItemsManager.Instance.KeyChange ();
							}
							else {
								ItemsManager.Instance.DungeonKeys -= 1;
								ItemsManager.Instance.DungeonKeyChange ();
							}
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

	public void Open()
	{
        if (OnChestOpen != null)
            OnChestOpen();

        //opens chest and plays animation
		animator.SetTrigger("Open");
		opened = true;

        if (containingItem || containingConsumables.Count > 0)
        {
            if (chestType == ChestType.Gold)
            {
                int random = Random.Range(0, 2);
                if (random == 0)
                    StartCoroutine(ReleaseItems());
                else
                    StartCoroutine(ReleaseObjects());
            }
            else if (chestType == ChestType.Iron)
                StartCoroutine(ReleaseConsumables());
            else if (chestType == ChestType.Dungeon)
                StartCoroutine(ReleaseItems(true));
        }

        SoundManager.PlaySound(openSound, transform.position);	

		//Remove icon from map once opened
		MapTracker icon = GetComponent<MapTracker>();
		if (icon)
			icon.Remove();

		EventSender events = GetComponentInParent<EventSender>();
        if (events)
            events.SendDisabledEvent();
	}

    void ReleaseParticle(Vector3 pos)
    {
        releaseParticle = ObjectPooler.GetPooledObject(releaseParticle);
        releaseParticle.transform.position = pos;
    }

    IEnumerator ReleaseObjects()
    {
        yield return new WaitForSeconds(releaseItemDelay);

		GameObject obj = (GameObject)Instantiate(containingObject, transform.position, Quaternion.Euler(0, 0, 0));

        if (obj.GetComponent<SwordStats>())
        {
            obj.AddComponent<DialogueSpeaker>();
            obj.AddComponent<ShowSwordStats>();

            obj.GetComponent<DialogueSpeaker>().playerLayer = playerMask;
            obj.GetComponent<DialogueSpeaker>().dialogueBoxPrefab = Resources.Load<GameObject>("PickupDialogueCanvas 1");
        }
        else if (obj.GetComponent<ShieldStats>())
        {
            obj.AddComponent<DialogueSpeaker>();
            obj.AddComponent<ShowShieldStats>();

            obj.GetComponent<DialogueSpeaker>().playerLayer = playerMask;
            obj.GetComponent<DialogueSpeaker>().dialogueBoxPrefab = Resources.Load<GameObject>("PickupDialogueCanvas 1");
        }

        obj.transform.position = transform.position;

        if (!animationBone)
            GetComponent<Rigidbody>().AddForce(Vector3.up * releaseItemForce, ForceMode.Impulse);
        else
        {
            animationBone.transform.Rotate(0, 180, 0);
            obj.transform.parent = animationBone.transform;
            animationBone.GetComponent<Animator>().SetTrigger("Animate");
        }

        ReleaseParticle(obj.transform.position);
    }

    IEnumerator ReleaseConsumables()
    {
        yield return new WaitForSeconds(releaseItemDelay);

        foreach (GameObject o in containingConsumables)
        {
            GameObject obj = ObjectPooler.GetPooledObject(o);
            //throw out of chest
            obj.transform.position = transform.position + Vector3.up;

            Vector3 direction = new Vector3(Random.insideUnitSphere.x, 1, Random.insideUnitSphere.z);
            GetComponent<Rigidbody>().AddForce(direction * releaseItemForce, ForceMode.Impulse);

		}
    }

	IEnumerator ReleaseItems(bool setParent = false)
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
            
            if (setParent)
                obj.transform.SetParent(transform, true);

            if (containingItem is Charm)
            {
                //Throw out of chest
                Rigidbody body = obj.GetComponent<Rigidbody>();
                if (body)
                    body.AddForce(Vector3.up * releaseItemForce, ForceMode.Impulse);
            }
            else
            {
                obj.transform.parent = animationBone.transform;

                obj.GetComponent<Collider>().enabled = false;

                animationBone.GetComponent<Animator>().SetTrigger("Animate");
            }
        }
            
        ReleaseParticle(obj.transform.position);

		PersistentObject persist = GetComponent<PersistentObject>();
		if (persist)
			persist.SetPersistentBool(false);

        yield return new WaitForSeconds(1);

        if (containingItem is InventoryItem)
            obj.GetComponent<Collider>().enabled = true;
	}
}
