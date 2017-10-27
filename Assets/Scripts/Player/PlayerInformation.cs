using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{

    [Header("Player Values")]
    public int playerIndex = 0;
    public float attackMinAngle = 130;
    public float attackDistance = 5;
    public float maxMoveSpeed;
    public float strength;
    public float attackSpeed;
    public float resistance;
    public float knockback;
    public float invincibilityTimeAfterHit = 2;
    public bool invincible = false;

    [Header("Charm")]
    public int charmAmount;
    public List<Charm> currentCharms = new List<Charm>();
    public Dictionary<ArmourType, Charm> currentItemCharms = new Dictionary<ArmourType, Charm>();

    [Header("Items")]
    public int itemAmount;
    public List<InventoryItem> currentItems = new List<InventoryItem>();

    [Header("Cure Orb Vals")]
    public int currentCureAmount = 0;
    public int maxCureAmount = 100;
    public int cureAmountUsedPerTick = 5;
    public AmountOfParticleTypes[] cureOrbParticles;

    [Header("Other Vals")]
    public float absorbDistance = 1;

    [Header("Average Damage Output")]
    public float damageOutput;

    public GameObject invincibleSphere;

    private Health health;

    private bool speedBoost = false;
    private bool paralysed = false;

    private float originalSpeed;
    private float originalResistance;

    private WeaponStats currentWeaponStats;
    [HideInInspector]
    public PlayerMove playerMove;
    [HideInInspector]
    public Animator animator;
    private CharacterController characterController;
    [HideInInspector]
    public PlayerAttack playerAttack;
    private GameObject mapHUD;

    private Dictionary<string, float> charmFloats = new Dictionary<string, float>();
    private Dictionary<string, bool> charmBools = new Dictionary<string, bool>();
    private Dictionary<string, float> itemFloats = new Dictionary<string, float>();
    private Dictionary<string, float> itemCharmFloats = new Dictionary<string, float>();
    private Dictionary<string, bool> itemCharmBools = new Dictionary<string, bool>();
    private LayerMask layerMask;

    void Start()
    {
        mapHUD = GameObject.FindGameObjectWithTag("MapHUD");
        originalSpeed = maxMoveSpeed;
        originalResistance = resistance;
        playerAttack = GetComponent<PlayerAttack>();
        health = GetComponent<Health>();
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();

        if (LevelGenerator.Instance)
        {
            LevelGenerator.Instance.OnTileEnter += RegenHealth;
            LevelGenerator.Instance.OnTileEnter += SpeedBuff;
            LevelGenerator.Instance.OnTileEnter += Forcefield;
            LevelGenerator.Instance.OnTileEnter += Paralysed;
        }

        PickupCharm(null);
    }

    void Update()
    {
        //sets the damage output
        damageOutput = GetSwordDamage();

        if (HasCharmBool("hideMap"))
        {
            mapHUD.SetActive(false);
        }
        else if (!mapHUD.gameObject.activeSelf)
        {
            mapHUD.SetActive(true);
        }
        //sets stats depending on weapon values
        if (currentWeaponStats)
        {
            //get weapon stats
        }
        else
        {
            currentWeaponStats = GetComponentInChildren<WeaponStats>();
        }

        //sets the players resistance
        if (resistance != originalResistance * GetCharmFloat("Resistance") * GetItemFloat("Resistance"))
        {
            resistance = originalResistance * GetCharmFloat("Resistance") * GetItemFloat("Resistance");
        }

        if (currentCureAmount < 0)
            currentCureAmount = 0;

        if (currentCureAmount > maxCureAmount)
            currentCureAmount = maxCureAmount;
			
        MagnetizeItems();
        PullEnemies();
    }

    public int ChanceChecker(string chanceKey)
    {
        if (HasCharmFloat(chanceKey))
        {
            float randomPercent = Random.Range(0, 101);
            if (randomPercent >= GetCharmFloat(chanceKey))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        return 0;
    }

    public void Revive(PlayerInformation deadPlayer, PlayerInformation alivePlayer)
    {
        //half the amount of alive players health
        float health = alivePlayer.health.health / 2;
        //set the alive players health
        alivePlayer.health.health = health;
        //call the event function
        alivePlayer.health.HealthChanged();

        //sets the dead players health
        deadPlayer.health.health = health;
        //call this event function
        deadPlayer.health.HealthChanged();
        //do particles and revive stuff
        //////////////////////////////particles
        deadPlayer.animator.SetBool("Die", false);
        //make sure player is no longer dead
        deadPlayer.health.isDead = false;
    }

    public void PickupCharm(Charm charm)
    {
        if (charm)
        {
            //adds a charm to the start of the list
            currentCharms.Insert(0, charm);
            //checks to see if the amount of charms the player has is greater then the amount they can hold
            if (currentCharms.Count > charmAmount)
            {
                //creates a new charm and adds it to the list
                Charm oldCharm = currentCharms[currentCharms.Count - 1];
                //removes the older charm
                currentCharms.Remove(oldCharm);

                oldCharm.Drop(this);
            }

            charm.Pickup(this);
        }
        CharmImage[] charmImg = FindObjectsOfType<CharmImage>();
        //loops through each charm and updates the ui
        foreach (CharmImage img in charmImg)
        {
            if (playerIndex == img.id)
                img.UpdateCharms(this);
        }
    }

    public void PickupItem(InventoryItem item)
    {
        if (item)
        {
            //checks to see if the amount of items the player has is greater then the amount they can hold
            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].armourType == item.armourType)
                {
                    //creates a new item and adds it to the list
                    InventoryItem oldItem = currentItems[currentItems.Count - 1];
                    //removes the older item
                    currentItems.Remove(oldItem);

                    //get tje current item charm that has the same enum
                    for (int j = 0; j < currentItemCharms.Count; j++)
                    {
                        if (currentItemCharms.ContainsKey(item.armourType))
                        {
                            Charm oldItemCharm = currentItemCharms[item.armourType];
                        }
                    }

                    currentItemCharms.Remove(item.armourType);
                    currentItemCharms.Add(item.armourType, item.charm);

                    if (oldItem.charm)
                        oldItem.charm.Drop(this);
                    oldItem.Drop(this);

                    break;
                }
            }

            //adds a item to the start of the list
            currentItems.Insert(0, item);

            item.Pickup(this);
        }
    }

    public void SetItemCharmFloat(string key, float value)
    {
        itemCharmFloats[key] = value;
    }

    public void SetItemCharmBool(string key, bool value)
    {
        itemCharmBools[key] = value;
    }

    public void SetItemFloat(string key, float value)
    {
        itemFloats[key] = value;
    }

    public float GetItemFloat(string key)
    {
        if (itemFloats.ContainsKey(key))
            return itemFloats[key];
        else
            return 1.0f;
    }

    public void RemoveItemCharmFloats(string key)
    {
        Debug.Log("removedCharmFloat");
        itemCharmFloats.Remove(key);
    }

    public void RemoveItemCharmBools(string key)
    {
        itemCharmBools.Remove(key);
    }

    public void RemoveCharmFloat(string key)
    {
        charmFloats.Remove(key);
    }

    public void RemoveCharmBool(string key)
    {
        charmBools.Remove(key);
    }

    public void SetLayerMask(LayerMask lm)
    {
        layerMask = lm;
    }

    public void SetCharmFloat(string key, float value)
    {
        charmFloats[key] = value;
    }

    public void SetCharmBool(string key, bool value)
    {
        charmBools[key] = value;
    }

    public float GetCharmFloat(string key)
    {
        if (charmFloats.ContainsKey(key))
            return charmFloats[key];
        else
        {
            if (itemCharmFloats.ContainsKey(key))
            {
                return itemCharmFloats[key];
            }
        }
            
        return 1.0f;
    }

    public bool GetCharmBool(string key)
    {
        if (charmBools.ContainsKey(key))
            return charmBools[key];
        else
        {
            if (itemCharmBools.ContainsKey(key))
            {
                return itemCharmBools[key];
            }
        }
            return false;
    }

    public bool HasCharmFloat(string key)
    {
        return charmFloats.ContainsKey(key);
    }

    public bool HasCharmBool(string key)
    {
        return charmBools.ContainsKey(key);
    }

    //-------------------------- Charm functions

    void MagnetizeItems()
    {
        //gets all items in radius
        Collider[] items = Physics.OverlapSphere(transform.position, (GetCharmFloat("radialValue") + absorbDistance));
        if (items.Length > 0)
        {
            foreach (Collider item in items)
            {
                //check the layer
                if (item.gameObject.layer == 15)
                {
                    if (item.GetComponent<PickupableItems>().canPickup)
                    {
                        //moves those items towards the player
                        item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, (GetCharmFloat("radialAbsorbSpeed") + 1) * Time.deltaTime);
                    }
                }
            }
        }
    }

    void PullEnemies()
    {
        if (HasCharmBool("pullEnemy"))
        {
                //gets the pull enemy charm
            foreach (Charm charm in currentCharms)
            {
                //gets all items in radius
                Collider[] enemies = Physics.OverlapSphere(transform.position, GetCharmFloat("radialValue"), layerMask);
                if (enemies.Length > 0)
                {
                    foreach (Collider enemy in enemies)
                    {
                        //moves those enemies towards the player
                        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, transform.position, GetCharmFloat("radialPullSpeed") * Time.deltaTime);
                    }
                }
            }
        }
    }

    private int roomAmount = 0;

    void RegenHealth()
    {
        if (HasCharmFloat("regenRooms"))
        {
            roomAmount++;

            int maxRoomAmount = Mathf.CeilToInt(GetCharmFloat("regenRooms"));

            if (roomAmount >= maxRoomAmount)
            {
                roomAmount = 0;
                health.health += GetCharmFloat("regenAmount");
            }
        }
    }

    void SpeedBuff()
    {
        if (!speedBoost)
        {
            if (HasCharmFloat("speedBuffTime"))
            {
                StartCoroutine(SpeedBuffForTime(GetCharmFloat("speedBuffTime"), GetCharmFloat("speedBuff")));
            }
        }
    }

    void Paralysed()
    {
        if (!paralysed)
        {
            if (HasCharmFloat("paralysisTime"))
            {
                StartCoroutine(ParalysedForSeconds(GetCharmFloat("paralysisTime")));
            }
        }
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        if (maxMoveSpeed != moveSpeed)
        {
            originalSpeed = maxMoveSpeed;
            maxMoveSpeed = moveSpeed;
        }
    }

    public void ResetMoveSpeed()
    {
        maxMoveSpeed = originalSpeed;
    }

    public float GetOriginalMoveSpeed()
    {
        return originalSpeed;
    }

    IEnumerator ParalysedForSeconds(float seconds)
    {
        paralysed = true;
        yield return new WaitForSeconds(.5f);
        animator.enabled = false;
        playerMove.enabled = false;
        yield return new WaitForSeconds(seconds);
        paralysed = false;
        animator.enabled = true;
        playerMove.enabled = true;
    }

    IEnumerator SpeedBuffForTime(float time, float multiplier)
    {
        speedBoost = true;
        float speed = maxMoveSpeed;
        maxMoveSpeed *= multiplier;

        yield return new WaitForSeconds(time);

        speedBoost = false;
        maxMoveSpeed = speed;
    }

    public void Forcefield()
    {
        if (HasCharmBool("firstHitInvincible"))
            invincibleSphere.SetActive(true);
    }

    public void KnockbackPlayer(Vector3 direction, float knockbackStrength)
    {
        StartCoroutine(KnockPlayerBack(direction, knockbackStrength));      
    }

    IEnumerator KnockPlayerBack(Vector3 direction, float knockbackStrength)
    {
        float elapsedTime = 0;
        float timer = .3f;
        while (elapsedTime < timer)
        {
            characterController.Move(direction * knockbackStrength * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 11)
        {
            //checks to make sure player has a charm with burn tick time
            if (HasCharmFloat("burnTickTime"))
            {
                if (col.transform.GetComponent<Health>())
                {
                    //the enemy can't be burned more then once, check to make sure it's not burned already
                    if (!col.gameObject.GetComponent<Health>().isBurned)
                    {
                        //set the enemy to burned with the following values
                        col.transform.GetComponent<Health>().SetBurned(GetCharmFloat("burnTickDamage"), GetCharmFloat("burnTickTotalTime"), GetCharmFloat("burnTickTime"));
                    }
                }
            }
            //checks to make sure player has a charm with poison tick time
            if (HasCharmFloat("poisonTickTime"))
            {
                if (col.transform.GetComponent<Health>())
                {
                    //the enemy can't be poisoned more then once, check to make sure it's not poisoned already
                    if (!col.gameObject.GetComponent<Health>().isPoisoned)
                    {
                        //set the enemy to poisoned with the following values
                        col.transform.GetComponent<Health>().SetPoison(GetCharmFloat("poisonTickDamage"), GetCharmFloat("poisonTickTotalTime"), GetCharmFloat("poisonTickTime"));
                    }
                }
            }
            //checks to make sure player has a charm with poison tick time
            if (HasCharmFloat("slowDeathTickTime"))
            {
                
                if (col.transform.GetComponent<Health>())
                {
                    //the enemy can't be poisoned more then once, check to make sure it's not poisoned already
                    if (!col.gameObject.GetComponent<Health>().isSlowlyDying)
                    {
                        //set the enemy to poisoned with the following values
                        col.transform.GetComponent<Health>().SetSlowDeath(GetCharmFloat("slowDeathTickDamage"), GetCharmFloat("slowDeathTickTotalTime"), GetCharmFloat("slowDeathTickTime"));
                    }
                }
            }
            //checks to make sure the player has a charm with damageOnTouch
            if (HasCharmFloat("damageOnTouch"))
            {
                if (col.transform.GetComponent<Health>())
                {
                    col.gameObject.GetComponent<Health>().AffectHealth(-GetCharmFloat("damageOnTouch"));
                    col.gameObject.GetComponent<Health>().Knockback(this, -col.transform.forward);
                }
            }
            if (HasCharmBool("touchKnockBack"))
            {
                if (col.transform.GetComponent<Health>())
                {
                    col.gameObject.GetComponent<Health>().Knockback(this, -col.transform.forward);
                    if (playerAttack.criticalHit() != 1)
                    {
                        col.gameObject.GetComponent<Health>().AffectHealth(-playerAttack.criticalHit());
                    }
                }
            }
        }
    }

    public float GetSwordDamage()
    {
        float specialAtkMultiplier = 1;
        //gets the multiplier for the attack type
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
            specialAtkMultiplier = playerAttack.spinDmgMultiplier;
        else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("DashAttack"))
            specialAtkMultiplier = playerAttack.dashAtkDmgMultiplier;
        else if (animator.GetCurrentAnimatorStateInfo(1).IsTag("RapidAttack"))
            specialAtkMultiplier = playerAttack.rapidDmgMultiplier;
        
        return (strength * playerAttack.sword.damageMultiplier * specialAtkMultiplier * GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit()) * GetCharmFloat("dmgMultiWhenBurned") * GetCharmFloat("dmgMultiWhenPoisoned") * CoinDamageMultiplier();
    }

    float CoinDamageMultiplier()
    {
        if (HasCharmFloat("multiplierPerCoin"))
        {
            return ItemsManager.Instance.Coins * GetCharmFloat("multiplierPerCoin");
        }
        else
        {
            return 1.0f;
        }
    }

}
