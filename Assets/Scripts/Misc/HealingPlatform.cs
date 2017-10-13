using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPlatform : MonoBehaviour {

    [Tooltip("Only have Player selected")]
    public LayerMask mask;
    public float timeBetweenHealthIncrease = 1;
    public AmountOfParticleTypes[] particles;
    public SoundEffect sound;

    private int counter = 0;
    private SpawnEffects spawnEffects;

	// Use this for initialization
	void Start () {
        spawnEffects = GameObject.FindObjectOfType<SpawnEffects>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, 10, mask);
        if (players.Length > 0)
        {
            foreach (Collider pl in players)
            {
                if (pl.GetComponent<Health>())
                {
                    if (counter > timeBetweenHealthIncrease * 60)
                    {
                        pl.GetComponent<Health>().health += .05f;
                        pl.GetComponent<Health>().HealthChanged();
                        spawnEffects.EffectOnHit(particles, transform.position);
                        SoundManager.PlaySound(sound, transform.position);
                        counter = 0;
                    }
                }
            }
        }
    }
}
