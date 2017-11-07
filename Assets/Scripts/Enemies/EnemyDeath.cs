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
    BossDrops
};

public class EnemyDeath : MonoBehaviour 
{
	public TypesOfDeath deathType;
    public AmountOfParticleTypes[] deathParticles;
    public SoundEffect deathSounds;
    public CameraShakeVars deathShake;

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
    [HideInInspector]
    public float shrinktime = .0005f;

	private Health health;
	private bool dead = false;
    private Drops enemyDrop;
    private Vector3 originalScale;

	// Use this for initialization
	void Awake () 
	{
        originalScale = transform.localScale;

        enemyDrop = GetComponent<Drops>();
		health = GetComponent<Health> ();
	}

    void OnEnable()
    {
        transform.localScale = originalScale;
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
            SplitEnemy();
        else if (deathType == TypesOfDeath.Nothing)
            Die();
        else if (deathType == TypesOfDeath.DamageExplode)
            DamageExplode();
        else if (deathType == TypesOfDeath.StatusExplode)
            StatusExplode();
        else if (deathType == TypesOfDeath.BossDrops)
            StartCoroutine(CreateBossDrop());

        Statistics.Instance.GetEnemy(health.enemyKind.enemyType);

        Statistics.Instance.enemiesKilled.Add(health.enemyKind);

        CameraShake.ShakeScreen(deathShake.magnitude, deathShake.shakeAmount, deathShake.duration);
	}

    IEnumerator CreateBossDrop()
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(.005f, .005f, .005f);
            enemyDrop.DoDrop();
            yield return new WaitForEndOfFrame();
        }
		Die ();
    }

	void SplitEnemy()
	{
		if (splitEnemy != null)
		{
			for (int i = 0; i < amountToSplit; i++) 
			{
				//create the split enemies and set them to this position
                GameObject enemy = ObjectPooler.GetPooledObject(splitEnemy);

                enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(new Vector3(transform.position.x, 0, transform.position.z));

                if (LevelGenerator.Instance)
                {
                    if (LevelGenerator.Instance.currentTile)
                    {
                        if (LevelGenerator.Instance.currentTile.GetComponentInChildren<EnemySpawner>())
                            LevelGenerator.Instance.currentTile.GetComponentInChildren<EnemySpawner>().spawnedEnemies.Add(enemy);     
                    }
                }
			}

            Die ();
		}
	}

    public void StatusExplode()
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

        if (health.health <= 0)
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
            SpawnEffects.EffectOnDeath(deathParticles, transform.position);

        CreateSoundObject();

        if (enemyDrop)
            enemyDrop.DoDrop();
            
        gameObject.SetActive(false);
	}
}
