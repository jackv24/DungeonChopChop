using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPlatform : MonoBehaviour {

    [Tooltip("Only have Player selected")]
    public LayerMask mask;
    public float healthIncreaseAmount = .1f;
    public float timeBetweenHealthIncrease = 1;
    public AmountOfParticleTypes[] particles;
    public SoundEffect sound;

    private int counter = 0;

    void FixedUpdate()
    {
        counter++;
        Collider[] players = Physics.OverlapSphere(transform.position, 1, mask);
        if (players.Length > 0)
        {
            foreach (Collider pl in players)
            {
                if (pl.GetComponent<Health>())
                {
                    if (pl.GetComponent<Health>().health < pl.GetComponent<Health>().maxHealth)
                    {
                        if (counter > timeBetweenHealthIncrease * 60)
                        {
                            pl.GetComponent<Health>().health += healthIncreaseAmount;
                            pl.GetComponent<Health>().HealthChanged();
                            SpawnEffects.EffectOnHit(particles, transform.position);
                            SoundManager.PlaySound(sound, transform.position);
                            counter = 0;
                        }
                    }
                }
            }
        }
    }
}
