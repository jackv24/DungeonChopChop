﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour 
{
	public float maxHealth;
	public float health;
    [Tooltip("The heavier the weight, less knockback")]
    public float weight;
	

	public delegate void HealthEvent();
	public event HealthEvent OnDeath;
	public event HealthEvent OnHealthChange;

    [Space()]
    public bool IsEnemy;
	public bool isPoisoned = false;
	public bool isBurned = false;
	public bool isSlowlyDying = false;

    public bool isDead = false;

    [Space()]
    public GameObject[] hitParticles;

    [Space()]
    [Header("Other Vals")]
    public float timeBetweenFlash = 0.1f;
    public int amountToFlash = 5;

	private PlayerInformation playerInfo;
	private Animator animator;
    private Rigidbody rb;

    private MeshRenderer[] renderers;
    private SkinnedMeshRenderer[] skinrends;

	public void AffectHealth(float healthDeta)
	{
		health += healthDeta;
		if (OnHealthChange != null) 
		{
			OnHealthChange ();
		}
		if (health > maxHealth) 
		{
			health = maxHealth;
		}
		if (health <= 0 && isDead == false) 
		{
			isDead = true;
			if (OnDeath != null) 
			{
				OnDeath ();
			}
		}
        if (IsEnemy)
        {
            if (animator)
            {
                animator.SetTrigger("GetHit");
            }
            DoHitParticle();
            HitFlash();
        }
	}

    void DoHitParticle()
    {
        if (hitParticles.Length > 0)
        {
            int random = Random.Range(0, hitParticles.Length);
            GameObject particle = ObjectPooler.GetPooledObject(hitParticles[random]);
            particle.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    void OnEnable()
    {
        if (IsEnemy)
        {
            EnableRenderers();
        }
        health = maxHealth;
        isDead = false;
    }

	void Start()
	{
        renderers = GetComponentsInChildren<MeshRenderer>();
        skinrends = GetComponentsInChildren<SkinnedMeshRenderer>();

        rb = GetComponent<Rigidbody>();
		OnHealthChange += TemporaryInvincibility;

		if (GetComponentInChildren<Animator> ())
		{
			animator = GetComponentInChildren<Animator> ();
		}
		if (GetComponent<PlayerInformation> ()) 
		{
			playerInfo = GetComponent<PlayerInformation> ();
		}
	}

	public void TemporaryInvincibility()
	{
		if (playerInfo)
		{
			StartCoroutine(InvincibilityWait(playerInfo));
		}
	}

	IEnumerator InvincibilityWait(PlayerInformation playerInfo)
	{
		playerInfo.invincible = true;
        HitFlash();
		yield return new WaitForSeconds(playerInfo.invincibilityTimeAfterHit);
		playerInfo.invincible = false;
	}

    public void Knockback(PlayerInformation playerInfo, Vector3 direction)
    {
        if (rb)
        {
            StartCoroutine(DisableNav(1));
            rb.AddForce(direction * (playerInfo.knockback / weight) * playerInfo.GetCharmFloat("kockbackMultiplier"), ForceMode.Impulse);
        }
    }

    IEnumerator DisableNav(float seconds)
    {
        GetComponent<EnemyMove>().usingNav = false;
        yield return new WaitForSeconds(seconds);
        GetComponent<EnemyMove>().usingNav = true;
    }

    public void HitFlash()
    {
        StartCoroutine(DoHitFlash());
    }

    IEnumerator DoHitFlash()
    {
        //Gets all mesh renderers and skin renderers
        for (int i = 0; i <= amountToFlash; i++)
        {
            DisableRenderers();
            yield return new WaitForSeconds(timeBetweenFlash);
            EnableRenderers();
            yield return new WaitForSeconds(timeBetweenFlash);
        }
    }

    void DisableRenderers()
    {
        //loops through each and disables them
        if (skinrends != null)
        {
            foreach (SkinnedMeshRenderer renderer in skinrends)
            {
                renderer.enabled = false;
            }
        }
        if (renderers != null)
        {
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }

    void EnableRenderers()
    {
        //loops through each and disables them
        if (skinrends != null)
        {
            foreach (SkinnedMeshRenderer renderer in skinrends)
            {
                renderer.enabled = true;
            }
        }
        if (renderers != null)
        {
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }
    }

	void Update()
	{
        //makes sure health doesn't go below 0        
		if (health <= 0) 
		{
			health = 0;
			isDead = true;
			Death ();
		}
		if (health > maxHealth) 
		{
			health = maxHealth;
		}
	}

	public void Damaged()
	{
		if (animator)
		{
			//animator.SetTrigger ("Hit");
		}
	}

    public bool HasStatusCondition()
    {
        if (isBurned || isPoisoned || isSlowlyDying)
        {
            return true;
        }
        return false;
    }
		
	public void Death()
	{
		//do death
		//checks if the game has an enemy drop script, if it does it is an enemy
		if (transform.GetComponent<EnemyDrops> ()) 
		{
			transform.GetComponent<EnemyDrops> ().DoDrop ();
		}
	}

	/// <summary>
	/// Sets the poison.
	/// </summary>
	/// <param name="duration">Duration in seconds.</param>
	/// <param name="timeBetweenPoison">Time between poison in seconds.</param>
	public void SetPoison(float damagePerTick, float duration, float timeBetweenPoison)
	{
		if (playerInfo)
			damagePerTick = playerInfo.GetCharmFloat ("poisonMultiplier");
		isPoisoned = true;
		StartCoroutine (doPoison (damagePerTick, duration, timeBetweenPoison));
	}

	IEnumerator doPoison(float damagePerTick, float duration, float timeBetweenPoison)
	{
		int counter = 0;
		while (isPoisoned) 
		{
			counter++;
			yield return new WaitForSeconds (timeBetweenPoison);
			AffectHealth (-damagePerTick);
			animator.SetTrigger ("Flinch");
			if (counter >= duration) 
			{
				isPoisoned = false;
			}
		}
	}

	/// <summary>
	/// Sets the burn.
	/// </summary>
	/// <param name="duration">Duration in seconds.</param>
	/// <param name="timeBetweenBurn">Time between poison in seconds.</param>
	public void SetBurned(float damagePerTick, float duration, float timeBetweenBurn)
	{
		if (playerInfo)
			damagePerTick = playerInfo.GetCharmFloat ("burnMultiplier");
		isBurned = true;
		StartCoroutine (doBurn (damagePerTick, duration, timeBetweenBurn));
	}

	IEnumerator doBurn(float damagePerTick, float duration, float timeBetweenBurn)
	{
		int counter = 0;
		while (isBurned) 
		{
			counter++;
			yield return new WaitForSeconds (timeBetweenBurn);
			AffectHealth (-damagePerTick);
			animator.SetTrigger ("Flinch");
			if (counter >= duration) 
			{
				isBurned = false;
			}
		}
	}

	/// <summary>
	/// Sets the slow death.
	/// </summary>
	/// <param name="duration">Duration in seconds.</param>
	/// <param name="timeBetweenDeathTick">Time between death tick in seconds.</param>
	public void SetSlowDeath(float damagePerTick, float duration, float timeBetweenDeathTick)
	{
		if (playerInfo)
			damagePerTick = playerInfo.GetCharmFloat ("deathTickMultiplier");
		isSlowlyDying = true;
		StartCoroutine (doSlowDeath (damagePerTick, duration, timeBetweenDeathTick));
	}

	IEnumerator doSlowDeath(float damagePerTick, float duration, float timeBetweenBurn)
	{
		int counter = 0;
		while (isSlowlyDying) 
		{
			counter++;
			yield return new WaitForSeconds (timeBetweenBurn);
			AffectHealth (-damagePerTick);
			animator.SetTrigger ("Flinch");
			if (counter >= duration) 
			{
				isSlowlyDying = false;
			}
		}
	}
}
