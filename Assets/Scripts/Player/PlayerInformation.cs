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

    [Header("Items")]
    public int itemAmount;
    public List<InventoryItem> currentItems = new List<InventoryItem>();

    public GameObject invincibleSphere;

    private Health health;

    private bool speedBoost = false;
    private bool paralysed = false;

    private float originalSpeed;

    private WeaponStats currentWeaponStats;
    private PlayerMove playerMove;
    private Animator animator;
    private Rigidbody rb;
    private CharacterController characterController;

    private Dictionary<string, float> charmFloats = new Dictionary<string, float>();
    private Dictionary<string, bool> charmBools = new Dictionary<string, bool>();
    private Dictionary<string, float> itemFloats = new Dictionary<string, float>();
    private LayerMask layerMask;

	void Start()
    {
        originalSpeed = maxMoveSpeed;
        health = GetComponent<Health>();
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
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

    public int chanceChecker(string chanceKey)
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

    void Update()
    {
        //sets stats depending on weapon values
        if (currentWeaponStats)
        {
            //get weapon stats
        }
        else
        {
            currentWeaponStats = GetComponentInChildren<WeaponStats>();
        }

        if (HasCharmFloat("resistanceMultiplier"))
            resistance = resistance * GetCharmFloat("resistanceMuliplier");
			
        MagnetizeItems();
        PullEnemies();
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
            //adds a item to the start of the list
            currentItems.Insert(0, item);
            //checks to see if the amount of items the player has is greater then the amount they can hold
            if (currentItems.Count > itemAmount)
            {
                //creates a new item and adds it to the list
                InventoryItem oldItem = currentItems[currentItems.Count - 1];
                //removes the older item
                currentItems.Remove(oldItem);

                oldItem.Drop(this);
            }

            item.Pickup(this);
        }
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
            return 1.0f;
    }

    public bool GetCharmBool(string key)
    {
        if (charmBools.ContainsKey(key))
            return charmBools[key];
        else
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
        Collider[] items = Physics.OverlapSphere(transform.position, (GetCharmFloat("radialValue") + 1));
        if (items.Length > 0)
        {
            foreach (Collider item in items)
            {
                //check the layer
                if (item.gameObject.layer == 15)
                {
                    //moves those items towards the player
                    item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, (GetCharmFloat("radialAbsorbSpeed") + 1) * Time.deltaTime);
                }
            }
        }
    }

    void PullEnemies()
    {
        if (HasCharmBool("pullEnemy"))
        {
            if (GetCharmBool("pullEnemy"))
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
        if (col.transform.tag == "Enemy")
        {
            //checks to make sure player has a charm with burn tick time
            if (HasCharmFloat("burnTickTime"))
            {
                //the enemy can't be burned more then once, check to make sure it's not burned already
                if (!col.gameObject.GetComponent<Health>().isBurned)
                {
                    if (col.transform.GetComponent<Health>())
                    {
                        //set the enemy to burned with the following values
                        col.transform.GetComponent<Health>().SetBurned(GetCharmFloat("burnTickDamage"), GetCharmFloat("burnTickTotalTime"), GetCharmFloat("burnTickTime"));
                    }
                }
            }
            //checks to make sure player has a charm with poison tick time
            if (HasCharmFloat("poisonTickTime"))
            {
                //the enemy can't be poisoned more then once, check to make sure it's not poisoned already
                if (!col.gameObject.GetComponent<Health>().isPoisoned)
                {
                    if (col.transform.GetComponent<Health>())
                    {
                        //set the enemy to poisoned with the following values
                        col.transform.GetComponent<Health>().SetPoison(GetCharmFloat("poisonTickDamage"), GetCharmFloat("poisonTickTotalTime"), GetCharmFloat("poisonTickTime"));
                    }
                }
            }
            //checks to make sure player has a charm with poison tick time
            if (HasCharmFloat("slowDeathTickTime"))
            {
                //the enemy can't be poisoned more then once, check to make sure it's not poisoned already
                if (!col.gameObject.GetComponent<Health>().isSlowlyDying)
                {
                    if (col.transform.GetComponent<Health>())
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
        }
    }

}
