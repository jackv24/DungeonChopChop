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
    public ParticleSystem fountainParticle;

    private int counter = 0;
    private float maxHealth = 10;
    private float maxCure = 100;

    private int maxParticleSize;
    private ParticleSystem.MainModule main;

    void Start()
    {
        if (fountainParticle)
            maxParticleSize = fountainParticle.main.maxParticles;

        InvokeRepeating("Heal", 0, timeBetweenHealthIncrease);
    }

    float Percentage(float number, int percent)
    {
        return ((float)number * percent) / 100;
    }

    void FixedUpdate()
    {
        if (fountainParticle)
        {
            main = fountainParticle.main;
        }
        //set the max health the player can heal to
        if (ItemsManager.Instance.hasGauntles)
        {
            maxHealth = 0;

            if (fountainParticle)
                main.maxParticles = (int)Percentage(maxParticleSize, 0);
        }
        else if (ItemsManager.Instance.hasArmourPiece)
        {
            maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 25);

            if (fountainParticle)
                main.maxParticles = (int)Percentage(maxParticleSize, 25);
        }
        else if (ItemsManager.Instance.hasBoots)
        {
            maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 50);

            if (fountainParticle)
                main.maxParticles = (int)Percentage(maxParticleSize, 50);
        }
        else if (ItemsManager.Instance.hasGoggles)
        {
            maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 75);

            if (fountainParticle)
                main.maxParticles = (int)Percentage(maxParticleSize, 75);
        }
    }

    void Heal()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, healingRadius, mask);
        if (players.Length > 0)
        {
            foreach (Collider pl in players)
            {
                if (pl.GetComponent<Health>())
                {
                    if (pl.GetComponent<Health>().health < maxHealth)
                    {
                        Health playerHealth = pl.GetComponent<Health>();
                        PlayerInformation playerInfo = pl.GetComponent<PlayerInformation>();

                        playerHealth.health += healthIncreaseAmount;
                        playerHealth.HealthChanged();

                        SpawnEffects.EffectOnHit(particles, transform.position);
                        SoundManager.PlaySound(sound, transform.position);
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
