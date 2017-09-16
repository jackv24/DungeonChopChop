using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDestroy : MonoBehaviour {

    public int hitAmount;
    [Tooltip("Amount of different particle types eg 'Dust, Smoke, Shrapnel'")]
    public AmountOfParticleTypes[] amountOfParticleTypes;

    private SpawnEffects spawnEffects;

	// Use this for initialization
	void Start () {
        spawnEffects = GameObject.FindObjectOfType<SpawnEffects>();
	}
	
	// Update is called once per frame
	void Update () {
        if (hitAmount <= 0)
        {
            //do effects
            if (spawnEffects)
                spawnEffects.EffectOnDeath(amountOfParticleTypes, transform.position);

            //do drop
            GetComponent<Drops>().DoDrop();

            gameObject.SetActive(false);
        }
	}

    public void DoEffect(Vector3 position)
    {
        if (spawnEffects)
            spawnEffects.EffectOnHit(amountOfParticleTypes, transform.position);
    }
}
