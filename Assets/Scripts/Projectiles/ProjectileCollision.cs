using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour {

	public int damageAmount;
    public GameObject[] hitParticles;

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
					if (playerInfo.HasCharmFloat ("immuneChance")) 
					{
						if (playerInfo.chanceChecker ("immuneChance") == 0) 
						{
							col.transform.GetComponent<Health> ().AffectHealth ((-damageAmount * damageMultiplyer / playerInfo.resistance));
						}
					} else 
					{
						col.transform.GetComponent<Health> ().AffectHealth ((-damageAmount * damageMultiplyer / playerInfo.resistance));
					}
				}
			} 
			//check to see if collider has an animator
			if (col.gameObject.GetComponentInChildren<Animator> ())
			{
				col.gameObject.GetComponentInChildren<Animator> ().SetTrigger ("Hit");
			}
			gameObject.SetActive (false);
		}
        DoParticle();
        gameObject.SetActive(false);
	}

    void DoParticle()
    {
        int random = Random.Range(0, hitParticles.Length);
        GameObject particle = ObjectPooler.GetPooledObject(hitParticles[random]);
        particle.transform.position = transform.position;
    }
}
