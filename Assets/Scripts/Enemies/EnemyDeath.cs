using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfDeath
{
	Nothing,
	SplitsIntoAnotherEnemy,
	CircleExplode,
    StatusExplode,
    DamageExplode,
};

public class EnemyDeath : MonoBehaviour 
{

	public TypesOfDeath deathType;

	//splits into enemy vals
	[HideInInspector]
	public int amountToSplit;
	[HideInInspector]
	public GameObject splitEnemy;
    [HideInInspector]
    [Tooltip("The radius of the explosion")]
    public float explodeRadius = 10;
    [HideInInspector]
    public StatusType statusType;
    [HideInInspector]
    public float damagePerTick = .5f;
    [HideInInspector]
    public float timeBetweenTick = 1;
    [HideInInspector]
    public float duration = 5;
    [HideInInspector]
    public float damageOnExplode = 2;
    [HideInInspector]
    public float knockbackOnExplode = 5;

    public AmountOfParticleTypes[] deathParticles;
    public SoundEffect deathSounds;

	private Health health;
	private bool dead = false;
    private SpawnEffects spawnEffects;
    private Drops enemyDrop;

	// Use this for initialization
	void Start () 
	{
        enemyDrop = GetComponent<Drops>();
        spawnEffects = GameObject.FindObjectOfType<SpawnEffects>();
		health = GetComponent<Health> ();
	}

    void OnEnable()
    {
        dead = false;
    }
	
	// Update is called once per frame
	void Update () 
	{
		if (health.isDead) 
		{
			if (!dead) 
			{
				//checks to make sure if dead
				dead = true;
				ChoseDeath ();
			}
		}
	}

	void ChoseDeath()
	{
		if (deathType == TypesOfDeath.SplitsIntoAnotherEnemy) 
		{
			SplitEnemy ();
		} 
		else if (deathType == TypesOfDeath.Nothing) 
		{
			Die ();
		}
        else if (deathType == TypesOfDeath.DamageExplode) 
        {
            DamageExplode();
        }
        else if (deathType == TypesOfDeath.StatusExplode) 
        {
            StatusExplode();
        }
	}

	void SplitEnemy()
	{
		if (splitEnemy != null)
		{
			for (int i = 0; i < amountToSplit; i++) 
			{
				//create the split enemies and set them to this position
				GameObject enemy = ObjectPooler.GetPooledObject (splitEnemy);
				enemy.transform.position = transform.position;
				Die ();
			}
		}
	}

    void StatusExplode()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, explodeRadius);
        List<GameObject> foundObjects = new List<GameObject>();
        foreach (Collider col in cols)
        {
            if (!foundObjects.Contains(col.gameObject))
            {
                foundObjects.Add(col.gameObject);
                if (col.GetComponent<Health>())
                {
                    if (col.GetComponent<Health>() != health)
                    {
                        if (statusType == StatusType.burn)
                            col.GetComponent<Health>().SetBurned(damagePerTick, duration, timeBetweenTick);
                        else if (statusType == StatusType.Ice)
                            col.GetComponent<Health>().SetIce(duration);
                        else if (statusType == StatusType.poison)
                            col.GetComponent<Health>().SetPoison(damagePerTick, duration, timeBetweenTick);
                        else if (statusType == StatusType.slowlyDying)
                            col.GetComponent<Health>().SetPoison(damagePerTick, duration, timeBetweenTick);

                        //do knockback
                        Vector3 direction = col.transform.position - transform.position;
                        if (col.GetComponent<PlayerInformation>())
                        {
                            col.GetComponent<PlayerInformation>().KnockbackPlayer(direction, knockbackOnExplode);
                        }
                        else
                        {
                            col.GetComponent<Health>().Knockback2(knockbackOnExplode, direction);
                        }
                    }
                }
            }
        }
        Die();
    }
    void DamageExplode()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, explodeRadius);
        List<GameObject> foundObjects = new List<GameObject>();
        //loop through each collider
        foreach (Collider col in cols)
        {
            if (!foundObjects.Contains(col.gameObject))
            {
                foundObjects.Add(col.gameObject);
                if (col.GetComponent<Health>())
                {
                    if (col.GetComponent<Health>() != health)
                    {
                        col.GetComponent<Health>().AffectHealth(-damageOnExplode);

                        //do knockback
                        Vector3 direction = col.transform.position - transform.position;
                        if (col.GetComponent<PlayerInformation>())
                        {
                            col.GetComponent<PlayerInformation>().KnockbackPlayer(direction, knockbackOnExplode);
                        }
                        else
                        {
                            col.GetComponent<Health>().Knockback2(knockbackOnExplode, direction);
                        }
                    }
                }
            }
        }
        Die();
    }


    void CreateSoundObject()
    {
        //play sound
        SoundManager.PlaySound(deathSounds, transform.position);
    }

	void Die()
	{
		//do die particles and stuff
        if (deathParticles.Length > 0)
            spawnEffects.EffectOnDeath(deathParticles, transform.position);

        CreateSoundObject();

        if (enemyDrop)
            enemyDrop.DoDrop();
            
        gameObject.SetActive(false);
	}
}
