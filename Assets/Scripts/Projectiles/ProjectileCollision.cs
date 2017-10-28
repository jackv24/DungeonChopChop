using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour {

	public int damageAmount;
    public float knockbackAmount;

    [Header("Particles and Sounds")]
    public AmountOfParticleTypes[] hitParticles;
    public SoundEffect hitSound;

	[HideInInspector]
	public float damageMultiplyer;
    [HideInInspector]
    public float thrust;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //make the projectile move
        rb.AddForce(transform.forward * thrust, ForceMode.Force); 
    }

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.GetComponent<Health>())
		{
			if (col.gameObject.GetComponent<PlayerInformation> ())
			{
				PlayerInformation playerInfo = col.gameObject.GetComponent<PlayerInformation> ();
                if (!playerInfo.invincible)
                {
                    if (playerInfo.HasCharmFloat("immuneChance"))
                    {
                        if (playerInfo.ChanceChecker("immuneChance") == 0)
                        {
                            if (!col.transform.GetComponent<PlayerAttack>().blocking)
                            {
                                col.transform.GetComponent<Health>().AffectHealth((-damageAmount * damageMultiplyer / playerInfo.resistance));
                            }
                            else
                            {
                                col.transform.GetComponent<Health>().AffectHealth((-damageAmount * damageMultiplyer / playerInfo.resistance / playerInfo.GetComponent<PlayerAttack>().shield.blockingResistance));
                            }
                        }
                    }
                    else
                    {
                        if (!col.transform.GetComponent<PlayerAttack>().blocking)
                        {
                            col.transform.GetComponent<Health>().AffectHealth((-damageAmount * damageMultiplyer / playerInfo.resistance));
                        } 
                        else 
                        {
                            //checks if the user is facing whatever the collision is coming from
                            float dot = Vector3.Dot(col.transform.forward, (transform.position - col.transform.position).normalized);
                            if (dot < 0.5f)
                            {
                                col.transform.GetComponent<Health>().AffectHealth(-damageAmount * damageMultiplyer / playerInfo.resistance);
                            }

                            SoundManager.PlaySound(playerInfo.playerAttack.hitBlockSound, transform.position);
                        }
                    }
                }
                //knockback player
                col.gameObject.GetComponent<PlayerInformation>().KnockbackPlayer(transform.forward, knockbackAmount);
			}
            else
            {
                col.transform.GetComponent<Health>().AffectHealth((-damageAmount * damageMultiplyer));
            }
		}

        DoSound();
        DoParticle();

        gameObject.SetActive(false);
	}

    void DoParticle()
    {
        SpawnEffects.EffectOnHit(hitParticles, transform.position);
    }

    void DoSound()
    {
        SoundManager.PlaySound(hitSound, transform.position);
    }
}
