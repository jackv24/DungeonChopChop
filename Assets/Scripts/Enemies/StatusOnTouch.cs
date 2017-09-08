using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum StatusType
{
    poison,
    burn,
    slowlyDying,
}

public class StatusOnTouch : MonoBehaviour {

    public StatusType statusType;
    public float damagePerTick;
    public float duration;
    public float timeBetweenDamage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.GetComponent<Health>())
        {
            if (!col.collider.GetComponent<Health>().HasStatusCondition())
            {
               if (statusType == StatusType.burn)
                    col.collider.GetComponent<Health>().SetBurned(damagePerTick, duration, timeBetweenDamage);
               else if (statusType == StatusType.poison)
                    col.gameObject.GetComponent<Health>().SetPoison(damagePerTick, duration, timeBetweenDamage);
               else if (statusType == StatusType.slowlyDying)
                   col.gameObject.GetComponent<Health>().SetSlowDeath(damagePerTick, duration, timeBetweenDamage);
            }
        }
    }
}
