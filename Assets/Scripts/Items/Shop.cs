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

    [Header("Stats Dialogue")]
    public Color equalColour = Color.white;
    public Color lessColour = Color.red;
    public Color moreColour = Color.green;
    [Space]
    public string swordDamageName = "Damage";
    public string swordRangeName = "Range";
    public string shieldResistanceName = "Strength";
    public string shieldSpeedDampingName = "Weight";

    private DialogueSpeaker speaker;
    private string dialogueText;

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

                if (sellingItem is InventoryItem)
                {
                    InventoryItem item = (InventoryItem)sellingItem;

                    if (item.usePrefabForPickup && item.itemPrefab)
                    {
                        //check what the item is by getting a specific script on it
                        if (item.itemPrefab.GetComponent<SwordStats>())
                        {
                            GameObject sword = ObjectPooler.GetPooledObject(item.itemPrefab, true);
                            playerInfo.playerAttack.AddSword(sword.GetComponent<SwordStats>());
                        } 
                        else if (item.itemPrefab.GetComponent<ShieldStats>())
                        {
                            GameObject shield = ObjectPooler.GetPooledObject(item.itemPrefab, true);
                            playerInfo.playerAttack.AddShield(shield.GetComponent<ShieldStats>());
                        }   
                        else if (item.itemPrefab.GetComponent<Orb>())
                        {
                            if (item.itemPrefab.GetComponent<Orb>().type == OrbType.Cure)
                            {
                                item.itemPrefab.GetComponent<Orb>().PickUpOrb(playerInfo);
                            }
                        }  
                    }

                    //pick up items
                    playerInfo.PickupItem((InventoryItem)sellingItem);
                }
                else if (sellingItem is Charm)
                {
                    if (LevelVars.Instance && LevelVars.Instance.droppedCharmPrefab)
                    {
                        GameObject obj = ObjectPooler.GetPooledObject(LevelVars.Instance.droppedCharmPrefab);

                        obj.transform.position = playerInfo.transform.position;

                        CharmPickup pickup = obj.GetComponentInChildren<CharmPickup>();
                        if (pickup)
                        {
                            pickup.representingCharm = (Charm)sellingItem;
                        }
                    }
                }

                sellingItem.Pickup(playerInfo);

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
            if (LevelGenerator.Instance && !LevelGenerator.Instance.IsFinished)
                LevelGenerator.Instance.OnGenerationFinished += SpawnMap;
            else
                SpawnMap();
        }

		if (!speaker)
		{
			speaker = GetComponent<DialogueSpeaker>();

			speaker.OnGetPlayer += AllowPurchase;
		}

		if(speaker && speaker.lines.Length > 0)
		{
			speaker.enabled = true;

            //make sure to remove old event first, so it is not assigned twice
            speaker.OnOpen -= UpdateSpeaker;
            speaker.OnOpen += UpdateSpeaker;

            //Dialogue box should only show piece of text - the shop text
            dialogueText = speaker.lines[0];
		}
	}

    void UpdateSpeaker(PlayerInformation player)
    {
        string descriptionText = "";

        if (sellingItem is InventoryItem)
        {
            InventoryItem it = (InventoryItem)sellingItem;

            //Item prefab might have sword or shield stats attached
            if (it.itemPrefab)
            {
                SwordStats swordStats = it.itemPrefab.GetComponent<SwordStats>();
                ShieldStats shieldStats = it.itemPrefab.GetComponent<ShieldStats>();

                //Display sword stats
                if (swordStats)
                {
                    //By default is assumed to be higher stats, since there may not currently be a sword
                    Color damageColour = moreColour;
                    Color rangeColour = moreColour;

                    SwordStats currentSword = player.playerAttack.sword;

                    //Set colour depending on if stats are better or worse than current
                    if (currentSword)
                    {
                        if (swordStats.damageMultiplier == currentSword.damageMultiplier)
                            damageColour = equalColour;
                        else if (swordStats.damageMultiplier < currentSword.damageMultiplier)
                            damageColour = lessColour;

                        if (swordStats.range == currentSword.range)
                            rangeColour = equalColour;
                        else if (swordStats.range < currentSword.range)
                            rangeColour = lessColour;
                    }

                    descriptionText += string.Format(
                        "<color=#{0}>{2}: {1}</color>\n",
                        ColorUtility.ToHtmlStringRGB(damageColour),
                        swordStats.damageMultiplier,
                        swordDamageName
                        );
                    descriptionText += string.Format(
                        "<color=#{0}>{2}: {1}</color>",
                        ColorUtility.ToHtmlStringRGB(rangeColour),
                        swordStats.range,
                        swordRangeName
                        );
                }
                
                //Display shield statse
                if (shieldStats)
                {
                    Color resistanceColour = moreColour;
                    Color speedColour = moreColour;

                    ShieldStats currentShield = player.playerAttack.shield;

                    if (currentShield)
                    {
                        if (shieldStats.blockingResistance == currentShield.blockingResistance)
                            resistanceColour = equalColour;
                        else if (shieldStats.blockingResistance < currentShield.blockingResistance)
                            resistanceColour = lessColour;

                        if (shieldStats.speedDamping == currentShield.speedDamping)
                            speedColour = equalColour;
                        else if (shieldStats.speedDamping < currentShield.speedDamping)
                            speedColour = lessColour;
                    }

                    descriptionText += string.Format(
                        "<color=#{0}>{2}: {1}</color>\n",
                        ColorUtility.ToHtmlStringRGB(resistanceColour),
                        shieldStats.blockingResistance,
                        shieldResistanceName
                        );
                    descriptionText += string.Format(
                        "<color=#{0}>{2}: {1}</color>",
                        ColorUtility.ToHtmlStringRGB(speedColour),
                        shieldStats.speedDamping,
                        shieldSpeedDampingName
                        );
                }
            }
        }

        //Update speaker to display one line
        speaker.lines = new string[] { string.Format(dialogueText, sellingItem.displayName, sellingItem.cost, descriptionText).Trim() };
    }

	void OnDestroy()
	{
        if (LevelGenerator.Instance)
        {
			LevelGenerator.Instance.OnGenerationFinished -= SpawnItem;
            LevelGenerator.Instance.OnGenerationFinished -= SpawnCharm;
			LevelGenerator.Instance.OnGenerationFinished -= SpawnMap;
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

	void SpawnMap()
	{
		MapItem it = (MapItem)sellingItem;

        if (itemSpawn && sellingItem.itemIcon)
        {
            itemGraphic = new GameObject("Sprite");
            itemGraphic.transform.SetParent(itemSpawn);
            itemGraphic.transform.localPosition = it.shopOffset.position;
            itemGraphic.transform.localEulerAngles = it.shopOffset.rotation;

			Vector3 scale = itemGraphic.transform.localScale;
            scale.x *= it.shopOffset.scale.x;
            scale.y *= it.shopOffset.scale.y;
            scale.z *= it.shopOffset.scale.z;
            itemGraphic.transform.localScale = scale;

            SpriteRenderer renderer = itemGraphic.AddComponent<SpriteRenderer>();

            renderer.sprite = sellingItem.itemIcon;
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
