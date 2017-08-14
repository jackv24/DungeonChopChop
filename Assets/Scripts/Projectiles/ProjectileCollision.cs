using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour {

	public int damageAmount;

	[HideInInspector]
	public float damageMultiplyer;

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
							col.transform.GetComponent<Health> ().AffectHealth ((int)(-damageAmount * damageMultiplyer));
						}
					} else 
					{
						col.transform.GetComponent<Health> ().AffectHealth ((int)(-damageAmount * damageMultiplyer));
					}
				}
			} else
			{
				col.transform.GetComponent<Health> ().AffectHealth ((int)(-damageAmount * damageMultiplyer));
			}
			//check to see if collider has an animator
			if (col.gameObject.GetComponentInChildren<Animator> ())
			{
				col.gameObject.GetComponentInChildren<Animator> ().SetTrigger ("Hit");
			}
			gameObject.SetActive (false);
		}
	}

}
