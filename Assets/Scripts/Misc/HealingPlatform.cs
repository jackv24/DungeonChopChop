using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPlatform : MonoBehaviour {

    [Tooltip("Only have Player selected")]
    public LayerMask mask;
    public float healthIncreaseAmount = .1f;
    public float timeBetweenIncrease = 1;
    public float nrgIncreaseAmount = 5;
    [Space()]
    public float healingRadius = 1;

    [Header("Audio and Sound")]
    public AmountOfParticleTypes[] healthParticles;
    public AmountOfParticleTypes[] nrgParticles;
    public SoundEffect sound;
    public ParticleSystem fountainParticle;

    [Space()]
    public bool isTownFountain = false;

    private int counter = 0;
    private float maxHealth = 10;
    private float maxNRG = 100;

    private int maxParticleSize;
    private ParticleSystem.MainModule main;

    void Start()
    {
        if (fountainParticle)
            maxParticleSize = fountainParticle.main.maxParticles;

        InvokeRepeating("Heal", 0, timeBetweenIncrease);
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

        if (isTownFountain)
        {
            //set the max health the player can heal to
            if (ItemsManager.Instance.hasGauntles)
            {
                maxHealth = 0;
                maxNRG = 0;

                if (fountainParticle)
                    main.maxParticles = (int)Percentage(maxParticleSize, 5);
            }
            else if (ItemsManager.Instance.hasArmourPiece)
            {
                maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 25);
                maxNRG = 25;

                if (fountainParticle)
                    main.maxParticles = (int)Percentage(maxParticleSize, 25);
            }
            else if (ItemsManager.Instance.hasBoots)
            {
                maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 50);
                maxNRG = 50;

                if (fountainParticle)
                    main.maxParticles = (int)Percentage(maxParticleSize, 50);
            }
            else if (ItemsManager.Instance.hasGoggles)
            {
                maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 75);
                maxNRG = 75;

                if (fountainParticle)
                    main.maxParticles = (int)Percentage(maxParticleSize, 75);
            }
            else
            {
                maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 100);
            }
        }
        else
        {
            maxHealth = Percentage(GameManager.Instance.players[0].playerMove.playerHealth.maxHealth, 100);
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

                        SpawnEffects.EffectOnHit(healthParticles, transform.position);
                        SoundManager.PlaySound(sound, transform.position);
                    }

                    if (pl.GetComponent<PlayerInformation>().currentCureAmount < maxNRG)
                    {
                        PlayerInformation playerInfo = pl.GetComponent<PlayerInformation>();

                        playerInfo.currentCureAmount += nrgIncreaseAmount;

                        SpawnEffects.EffectOnHit(nrgParticles, transform.position);
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
