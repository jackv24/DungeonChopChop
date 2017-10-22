using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class ImmuneTo : MonoBehaviour {

    [Header("Enemy Type")]
    public StatusType type;

    private Health health;

	// Use this for initialization
	void Start () {
        health = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update () {
        if (health.HasStatusCondition())
        {
            if (type == StatusType.burn)
                health.isBurned = false;
            else if (type == StatusType.Ice)
                health.isFrozen = false;
            else if (type == StatusType.poison)
                health.isPoisoned = false;
            else if (type == StatusType.Sandy)
                health.isSandy = false;
            else if (type == StatusType.slowlyDying)
                health.isSlowlyDying = false;
        }
	}
}
