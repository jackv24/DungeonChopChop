using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tenticles : MonoBehaviour {

    public int healthPoints = 3;
    public float DamageOnTouch = .2f;
    [Tooltip("'Hit' == When they get hit, 'Death' == When they get destroyed")]
    public AmountOfParticleTypes[] particleTypes;
    private Quicksand quicksand;

    private int OGHealthPoints = 0;

    void OnEnable()
    {
        healthPoints = OGHealthPoints;
    }

	// Use this for initialization
	void Awake () {
        OGHealthPoints = healthPoints;
        quicksand = GetComponentInParent<Quicksand>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Health>())
            col.GetComponent<Health>().AffectHealth(-.2f);

        if (col.gameObject.layer == 16)
        {
            BoxCollider box = col.GetComponent<BoxCollider>();
            if (box.enabled)
            {
                SpawnEffects.EffectOnHit(particleTypes, transform.position);
                healthPoints--;
                if (healthPoints <= 0)
                {
                    SpawnEffects.EffectOnDeath(particleTypes, transform.position);
                    quicksand.SpikesHit();
                }
            }
        }
    }
}
