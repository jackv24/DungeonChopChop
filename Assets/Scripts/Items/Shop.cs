using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	public Transform itemSpawn;

	public BaseItem sellingItem;
	private GameObject itemGraphic;

	[Space()]
	public SoundEffect purchaseSound;
    private AudioSource audioSource;

    [Space()]
    public GameObject purchaseEffect;

    private DialogueSpeaker speaker;

	private bool canPurchase = false;
	private PlayerInputs input;
	private PlayerInformation playerInfo;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

		if(!InputManager.Instance)
		{
			input = new PlayerInputs();
			input.AddControllerBindings();
			input.AddKeyboardBindings();
		}
    }

    void PlaySound()
    {
		SoundManager.PlaySound(purchaseSound, transform.position);
    }

	void Update()
	{
		if(canPurchase && sellingItem && input != null && input.Purchase.WasPressed)
		{
			if(ItemsManager.Instance.Coins >= sellingItem.cost)
			{
				ItemsManager.Instance.Coins -= sellingItem.cost;

                PlaySound();

                //Spawn particle effect
                if (purchaseEffect)
                {
                    GameObject purchaseEffectObj = ObjectPooler.GetPooledObject(purchaseEffect);
                    if (purchaseEffectObj)
                        purchaseEffectObj.transform.position = itemSpawn.position;
                }

                sellingItem.Pickup(playerInfo);

				if (sellingItem is InventoryItem)
				{
					InventoryItem item = (InventoryItem)sellingItem;

					if (item.usePrefabForPickup && item.itemPrefab)
					{
						GameObject obj = ObjectPooler.GetPooledObject(item.itemPrefab);
						obj.transform.position = playerInfo.transform.position;
					}
				}
				else if(sellingItem is Charm)
				{
					if (LevelVars.Instance && LevelVars.Instance.droppedCharmPrefab)
					{
						GameObject obj = ObjectPooler.GetPooledObject(LevelVars.Instance.droppedCharmPrefab);

						obj.transform.position = playerInfo.transform.position;

						CharmPickup pickup = obj.GetComponentInChildren<CharmPickup>();
						if (pickup)
						{
							pickup.representingCharm = (Charm)sellingItem;
							pickup.Pickup(playerInfo);
						}
					}
				}

				Destroy(itemGraphic);
				sellingItem = null;

				speaker.Close(true);
			}
		}
	}

	public void SpawnItem(BaseItem item)
	{
		sellingItem = item;

		if (item is InventoryItem)
		{
			if (LevelGenerator.Instance && !LevelGenerator.Instance.IsFinished)
                LevelGenerator.Instance.OnGenerationFinished += SpawnItem;
            else
                SpawnItem();
		}
		else if(item is Charm)
		{
			if (LevelGenerator.Instance && !LevelGenerator.Instance.IsFinished)
				LevelGenerator.Instance.OnGenerationFinished += SpawnCharm;
			else
				SpawnCharm();
		}
		else if(item is MapItem)
		{
            if (itemSpawn && item.itemIcon)
            {
                itemGraphic = new GameObject("Sprite");
                itemGraphic.transform.SetParent(itemSpawn);
                itemGraphic.transform.localPosition = Vector3.zero;
                itemGraphic.transform.eulerAngles = new Vector3(60, 0, 0);

                SpriteRenderer renderer = itemGraphic.AddComponent<SpriteRenderer>();

                renderer.sprite = item.itemIcon;
            }
        }

		if (!speaker)
		{
			speaker = GetComponent<DialogueSpeaker>();

			speaker.OnGetPlayer += AllowPurchase;
		}

		if(speaker && speaker.lines.Length > 0)
		{
			speaker.enabled = true;

			//Dialogue box should only show piece of text - the shop text
			string text = speaker.lines[0];
			speaker.lines = new string[] { string.Format(text, item.displayName, item.cost) };
		}
	}

	void OnDestroy()
	{
        if (LevelGenerator.Instance)
        {
			LevelGenerator.Instance.OnGenerationFinished -= SpawnItem;
            LevelGenerator.Instance.OnGenerationFinished -= SpawnCharm;
        }
    }

	void SpawnItem()
	{
		InventoryItem it = (InventoryItem)sellingItem;

        if (itemSpawn && it.itemPrefab)
        {
            itemGraphic = Instantiate(it.itemPrefab, itemSpawn);
            itemGraphic.transform.localPosition = it.shopOffset.position;
            itemGraphic.transform.localEulerAngles = it.shopOffset.rotation;

            Vector3 scale = itemGraphic.transform.localScale;
            scale.x *= it.shopOffset.scale.x;
			scale.y *= it.shopOffset.scale.y;
			scale.z *= it.shopOffset.scale.z;
            itemGraphic.transform.localScale = scale;

            //Only need to display this item, don't need any behaviours
            Component[] components = itemGraphic.GetComponentsInChildren<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (!(components[i] is MeshRenderer
				|| components[i] is MeshFilter
				|| components[i] is Transform
				|| components[i] is ParticleSystem
				|| components[i] is ParticleSystemRenderer))
                    DestroyImmediate(components[i], false);
            }
        }
	}

	void SpawnCharm()
	{
		if (LevelVars.Instance && LevelVars.Instance.droppedCharmPrefab)
		{
			itemGraphic = Instantiate(LevelVars.Instance.droppedCharmPrefab, itemSpawn);
			itemGraphic.transform.localPosition = Vector3.zero;

			CharmPickup pickup = itemGraphic.GetComponentInChildren<CharmPickup>();
			if (pickup)
				pickup.representingCharm = (Charm)sellingItem;

			//Only need to display this item, don't need any behaviours
			Component[] components = itemGraphic.GetComponentsInChildren<Component>();
			for (int i = components.Length - 1; i >= 0; i--)
			{
				if (components[i] is Collider || components[i] is Rigidbody)
					DestroyImmediate(components[i], false);
			}
		}
	}

	void AllowPurchase(PlayerInformation playerInfo, bool value)
	{
		canPurchase = value;
		this.playerInfo = playerInfo;

		if(InputManager.Instance)
			input = InputManager.GetPlayerInput(playerInfo.playerIndex);
	}
}
