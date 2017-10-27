using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPlatform : MonoBehaviour {

    [Tooltip("Only have Player selected")]
    public LayerMask mask;
    public float healthIncreaseAmount = .1f;
    public int cureAmountIncrease = 5;
    public float timeBetweenHealthIncrease = 1;
    public float healingRadius = 1;
    public AmountOfParticleTypes[] particles;
    public SoundEffect sound;

    private int counter = 0;

    void FixedUpdate()
    {
        counter++;
        Collider[] players = Physics.OverlapSphere(transform.position, healingRadius, mask);
        if (players.Length > 0)
        {
            foreach (Collider pl in players)
            {
                if (pl.GetComponent<Health>())
                {
                    if (pl.GetComponent<Health>().health < pl.GetComponent<Health>().maxHealth || pl.GetComponent<PlayerInformation>().currentCureAmount < pl.GetComponent<PlayerInformation>().maxCureAmount)
                    {
                        if (counter > timeBetweenHealthIncrease * 60)
                        {
                            Health playerHealth = pl.GetComponent<Health>();
                            PlayerInformation playerInfo = pl.GetComponent<PlayerInformation>();

                            playerHealth.health += healthIncreaseAmount;
                            playerHealth.HealthChanged();

                            playerInfo.currentCureAmount += cureAmountIncrease;

                            SpawnEffects.EffectOnHit(particles, transform.position);
                            SoundManager.PlaySound(sound, transform.position);
                            counter = 0;
                        }
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, healingRadius);
    }
}
