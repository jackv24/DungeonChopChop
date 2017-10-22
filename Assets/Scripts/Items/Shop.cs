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
    public GameObject itemPurchasePopup;

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
	private PlayerInformation playerInfo;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound()
    {
		SoundManager.PlaySound(purchaseSound, transform.position);
    }

	void Update()
	{
        if(canPurchase && sellingItem && playerInfo.playerMove.input.Purchase.WasPressed)
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

                    if(itemPurchasePopup)
                    {
                        GameObject obj = ObjectPooler.GetPooledObject(itemPurchasePopup);

                        ShopItemPopup popup = obj.GetComponent<ShopItemPopup>();
                        if (popup)
                            popup.Init(item, playerInfo.transform);
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
                            pickup.Pickup(playerInfo);
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

                //creates a string for both values on sword and shield, they are there to add '+' or '-'
                string var1String = "";
                string var2String = "";

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
                        //gets the damage difference between current and the sword in the shop
                        float damageDifference = Mathf.Abs(swordStats.damageMultiplier - currentSword.damageMultiplier);

                        //gets the range difference between current and the sword in the shop
                        float rangeDifference = Mathf.Abs(swordStats.range - currentSword.range);

                        if (swordStats.damageMultiplier == currentSword.damageMultiplier)
                        {
                            damageColour = equalColour;
                            var1String = "" + 0;
                        }
                        else if (swordStats.damageMultiplier < currentSword.damageMultiplier)
                        {
                            damageColour = lessColour;
                            var1String = "-" + damageDifference;
                        }
                        else if (swordStats.damageMultiplier > currentSword.damageMultiplier)
                        {
                            var1String = "+" + damageDifference;
                        }

                        if (swordStats.range == currentSword.range)
                        {
                            rangeColour = equalColour;
                            var2String = "" + 0;
                        }
                        else if (swordStats.range < currentSword.range)
                        {
                            rangeColour = lessColour;
                            var2String = "-" + rangeDifference;
                        }
                        else if (swordStats.range > currentSword.range)
                        {
                            var2String = "+" + rangeDifference;
                        }
                    }

                    descriptionText += string.Format(
                        "{2}: <color=#{0}>{1}</color>\n", 
                        ColorUtility.ToHtmlStringRGB(damageColour), 
                        var1String, 
                        swordDamageName
                        );

                    descriptionText += string.Format(
                        "{2}: <color=#{0}>{1}</color>\n",
                        ColorUtility.ToHtmlStringRGB(rangeColour),
                        var2String,
                        swordRangeName
                        );

                    //If the weapon has an effect, set the effect colour

                    Color effectColor = equalColour;

                    var1String = "";

                    if (swordStats.weaponEffect == WeaponEffect.Burn)
                    {
                        var1String = "Burns";
                        effectColor = Color.red;
                    }
                    else if (swordStats.weaponEffect == WeaponEffect.Ice)
                    {
                        var1String = "Freezes";
                        effectColor = Color.blue;
                    }
                    else if (swordStats.weaponEffect == WeaponEffect.Poison)
                    {
                        var1String = "Poisons";
                        effectColor = Color.magenta;
                    }
                    else if (swordStats.weaponEffect == WeaponEffect.Sandy)
                    {
                        var1String = "Sandy";
                        effectColor = Color.yellow;
                    }
                    else if (swordStats.weaponEffect == WeaponEffect.SlowDeath)
                    {
                        var1String = "Infects";
                        effectColor = Color.green;
                    }

                    if (var1String.Length > 0)
                    {
                        descriptionText += string.Format(
                            "<color=#{0}>{1}</color>\n",
                            ColorUtility.ToHtmlStringRGB(effectColor),
                            var1String
                            );
                    }
                }
                
                //Display shield statse
                if (shieldStats)
                {
                    Color resistanceColour = moreColour;
                    Color speedColour = moreColour;

                    ShieldStats currentShield = player.playerAttack.shield;

                    if (currentShield)
                    {
                        //gets the blocking difference between current and the shield in the shop
                        float blockingDifference = Mathf.Abs(currentShield.blockingResistance - shieldStats.blockingResistance);

                        //gets the speed difference between current and the shield in the shop
                        float speedDifference = Mathf.Abs(currentShield.speedDamping - shieldStats.speedDamping);

                        if (shieldStats.blockingResistance == currentShield.blockingResistance)
                        {
                            resistanceColour = equalColour;
                            var1String = "" + 0;
                        }
                        else if (shieldStats.blockingResistance < currentShield.blockingResistance)
                        {
                            resistanceColour = lessColour;
                            var1String = "-" + blockingDifference;
                        }
                        else if (shieldStats.blockingResistance > currentShield.blockingResistance)
                        {
                            var1String = "+" + blockingDifference;
                        }

                        if (shieldStats.speedDamping == currentShield.speedDamping)
                        {
                            speedColour = equalColour;
                            var2String = "" + 0;
                        }
                        else if (shieldStats.speedDamping < currentShield.speedDamping)
                        {
                            speedColour = lessColour;
                            var2String = "-" + speedDifference;
                        }
                        else if (shieldStats.speedDamping > currentShield.speedDamping)
                        {
                            speedColour = lessColour;
                            var2String = "+" + speedDifference;
                        }
                    } 
                    else 
                    {
                        var1String = "+" + shieldStats.blockingResistance;
                        var2String = "+" + shieldStats.speedDamping;
                    }

                    descriptionText += string.Format(
                        "{2}: <color=#{0}>{1}</color>\n",
                        ColorUtility.ToHtmlStringRGB(resistanceColour),
                        var1String,
                        shieldResistanceName
                        );

                    descriptionText += string.Format(
                        "{2}: <color=#{0}>{1}</color>\n",
                        ColorUtility.ToHtmlStringRGB(speedColour),
                        var2String,
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
	}
}
