﻿using System.Collections;
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
    public int releaseSphereSize = 3;

    [Header("Audio")]
    public SoundEffect openSound;

    [Header("Particles")]
    public GameObject releaseParticle;
    public AmountOfParticleTypes[] poofParticle;

    private Animator animator;
    private EnemySpawner enemySpawner;

    void OnEnable()
    {
        if (chestType == ChestType.Iron)
            SpawnEffects.EffectOnHit(poofParticle, transform.position);
    }

	void Start ()
	{
        animator = GetComponentInChildren<Animator>();

        enemySpawner = GetComponentInParent<EnemySpawner>();

        //populate the chest with gold items
        if (chestType == ChestType.Gold)
        {
            if (randomise)
            {
                if (possibleItems.Length > 0)
                    containingItem = Helper.GetRandomItemByProbability(possibleItems);
                if (possibleObjects.Length > 0)
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
                    if (possibleConsumables.Length > 0)
                    {
                        GameObject obj = Helper.GetRandomGameObjectByProbability(possibleConsumables);
                        containingConsumables.Add(obj);
                    }
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

                                if (enemySpawner)
                                    enemySpawner.cleared = false;
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

    void RemoveChest()
    {
        Collider[] cols = GetComponentsInChildren<Collider>();
        MeshRenderer[] rends = GetComponentsInChildren<MeshRenderer>();

        GetComponentInChildren<Animator>().enabled = false;

        foreach (Collider col in cols)
        {
            if (col.transform.parent)
            {
                if (col.transform.parent.name != "RootAnimation")
                    col.enabled = false;
            }
            else
                col.enabled = false;
        }

        foreach (MeshRenderer ren in rends)
        {
            if (ren.transform.parent.name != "RootAnimation")
                ren.enabled = false;
        }

        SpawnEffects.EffectOnHit(poofParticle, transform.position);
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
                //release charm or sword

                //int random = Random.Range(0, 2);
                //if (random == 0)
                StartCoroutine(ReleaseItems());
                //else
                //    StartCoroutine(ReleaseObjects());
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

		releaseParticle.transform.parent = transform;
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

        RemoveChest();

    }

    IEnumerator ReleaseConsumables()
    {
        yield return new WaitForSeconds(releaseItemDelay);

        foreach (GameObject o in containingConsumables)
        {
            GameObject obj = ObjectPooler.GetPooledObject(o);
            //throw out of chest

            Vector3 direction = new Vector3(Random.insideUnitSphere.x * releaseSphereSize, Random.insideUnitSphere.y * releaseSphereSize, Random.insideUnitSphere.z * releaseSphereSize);

            if (obj)
            {
                obj.transform.position = transform.position + Vector3.up;
                obj.GetComponent<Rigidbody>().AddForce(direction * releaseItemForce, ForceMode.Impulse);
            }
		}

        RemoveChest();
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

            enemySpawner.Spawn(true);
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

                RemoveChest();
            }
            else
            {
                obj.transform.parent = animationBone.transform;

                animationBone.GetComponent<Animator>().SetTrigger("Animate");
            }
        }
            
        ReleaseParticle(obj.transform.position);

		PersistentObject persist = GetComponent<PersistentObject>();
		if (persist)
			persist.SetPersistentBool(false);
	}
}
