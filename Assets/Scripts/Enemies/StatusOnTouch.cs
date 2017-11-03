using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum StatusType
{
    poison,
    burn,
    slowlyDying,
    Ice,
    Sandy
}

public class StatusOnTouch : MonoBehaviour {

    public StatusType statusType;
    public float damagePerTick;
    public float duration;
    public float timeBetweenDamage;
    public float speedDamper;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Collide(Collider col)
    {
        if (col.GetComponent<Collider>().GetComponent<Health>())
        {
            if (!col.GetComponent<Collider>().GetComponent<Health>().HasStatusCondition())
            {
                if (col.GetComponent<Collider>().GetComponent<PlayerAttack>())
                {
                    if (!col.GetComponent<Collider>().GetComponent<PlayerAttack>().blocking)
                    {
                        if (statusType == StatusType.burn)
                            col.GetComponent<Collider>().GetComponent<Health>().SetBurned(damagePerTick, duration, timeBetweenDamage);
                        else if (statusType == StatusType.poison)
                            col.gameObject.GetComponent<Health>().SetPoison(damagePerTick, duration, timeBetweenDamage);
                        else if (statusType == StatusType.slowlyDying)
                            col.gameObject.GetComponent<Health>().SetSlowDeath(damagePerTick, duration, timeBetweenDamage);
                        else if (statusType == StatusType.Ice)
                            col.gameObject.GetComponent<Health>().SetIce(duration);
                        else if (statusType == StatusType.Sandy)
                            col.gameObject.GetComponent<Health>().SetSandy(duration, speedDamper);
                    }
                }
                else if (col.GetComponent<Collider>().GetComponent<PropDestroy>())
                {
                    if (statusType == StatusType.burn)
                        col.GetComponent<Collider>().GetComponent<Health>().SetBurned(damagePerTick, duration, timeBetweenDamage);
                }
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Collide(col.collider);
    }

    void OnTriggerEnter(Collider col)
    {
        Collide(col);
    }
}
