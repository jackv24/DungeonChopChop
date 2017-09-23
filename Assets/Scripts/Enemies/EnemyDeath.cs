using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfDeath
{
	Nothing,
	SplitsIntoAnotherEnemy,
	CircleExplode,
};

public class EnemyDeath : MonoBehaviour 
{

	public TypesOfDeath deathType;

	//splits into enemy vals
	[HideInInspector]
	public int amountToSplit;
	[HideInInspector]
	public GameObject splitEnemy;
    public AmountOfParticleTypes[] deathParticles;
    public AudioClip[] deathSounds;

	private Health health;
	private bool dead = false;
    private SpawnEffects spawnEffects;
    private AudioSource AS;
    private Drops enemyDrop;

	// Use this for initialization
	void Start () 
	{
        enemyDrop = GetComponent<Drops>();
        AS = GetComponent<AudioSource>();
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

    void CreateSoundObject()
    {
        //play sound
        GameObject obj = new GameObject();
        obj.transform.position = transform.position;
        obj.AddComponent<AudioSource>();
        int random = Random.Range(0, deathSounds.Length);
        obj.GetComponent<AudioSource>().volume = AS.volume;
        obj.GetComponent<AudioSource>().PlayOneShot(deathSounds[random]);
        obj.AddComponent<SoundObject>();
    }

	void Die()
	{
		//do die particles and stuff
        if (deathParticles.Length > 0)
            spawnEffects.EffectOnDeath(deathParticles, transform.position);

        if (deathSounds.Length > 0)
        {
            CreateSoundObject();
        }

        if (enemyDrop)
            enemyDrop.DoDrop();
            
        gameObject.SetActive(false);
	}
}
