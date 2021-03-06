﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TypesOfAttack
{
    Nothing,
    BasicShootIntervals,
    BasicShootRandIntervals,
    ShootCircleIntervals,
    ShootCircleRandIntervals,
    BigBossAttack
};

public class EnemyAttack : MonoBehaviour
{
    struct Colliding
    {
        public Collider col;
        public Health health;
    }


    public TypesOfAttack attackingType;
    public bool damagesOtherEnemies = true;

    [Header("Projectile Vars")]
    [HideInInspector]
    public GameObject projecticle;
    [HideInInspector]
    public GameObject shootPosition;
    [HideInInspector]
    public float thrust;
    [HideInInspector]
    public float projectileDmgMutliplier = 1;

    [Space()]
    [Tooltip("This values x projectile damage")]
    public float damageOnTouch;
    public float knockbackStrength;

    [Header("Audio")]
    public SoundEffect shootSounds;

    //shoot circle vars
    [HideInInspector]
    public int projAmount;

    //interval vars
    [HideInInspector]
    public float timeTillInterval;

    //shoot random vars
    [HideInInspector]
    public float minInterval;
    [HideInInspector]
    public float maxInterval;

    [HideInInspector]
    public bool burstFire;
    [HideInInspector]
    public int burstAmount;
    [HideInInspector]
    public float timeBetweenShots;

    private float shootIntervalCounter = 0;
    private int circleAngle = 0;
    private float angle = 0;
    private float randomInterval = 0;
    private float originalStrength;
    private float originalHealth;

    protected EnemyMove enemyMove;
    protected Animator animator;
    protected Health enemyHealth;
    protected EnemyDeath enemyDeath;
    protected NavMeshAgent agent;

    private FloorSpikes floorSpikes;

    [HideInInspector]
    public bool usesChildRotation = false;

    private Collider col;

    private List<Colliding> colliding = new List<Colliding>();

    LevelTile parentTile = null;

    void Start()
    {
        if (projectileDmgMutliplier == 0)
            projectileDmgMutliplier = 1;

        floorSpikes = GetComponentInParent<FloorSpikes>();

        enemyDeath = GetComponent<EnemyDeath>();
        col = GetComponent<Collider>();

        enemyHealth = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
        enemyMove = GetComponent<EnemyMove>();

        agent = GetComponent<NavMeshAgent>();

        //If this enemy was spawned as a child of a tile, only enable when tile is entered
        parentTile = GetComponentInParent<LevelTile>();

        if(parentTile)
        {
            parentTile.OnTileEnter += SetActive;
            parentTile.OnTileExit += SetActive;

            SetInactive();
        }

        if (enemyHealth)
            originalHealth = enemyHealth.maxHealth;
    }

    void OnEnable()
    {
        colliding.Clear();
    }

    public void ChangeHealth()
    {
        if (enemyHealth)
        {
            enemyHealth.maxHealth = originalHealth * GameManager.Instance.enemyHealthMultiplier;
            enemyHealth.health = enemyHealth.maxHealth;
        }
    }

    public void ChangeStrength()
    {
		damageOnTouch =  damageOnTouch * GameManager.Instance.enemyStrengthMultiplier;
    }

    void SetActive()
    {
        gameObject.SetActive(true);
    }

    void SetInactive()
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (parentTile)
        {
            parentTile.OnTileEnter -= SetActive;
            parentTile.OnTileExit -= SetActive;
        }
    }

    void ChoseAttackType()
    {
        randomInterval = Random.Range(minInterval, maxInterval);
        if (attackingType == TypesOfAttack.BasicShootIntervals)
        {
            BasicShootIntervals();
        }
        else if (attackingType == TypesOfAttack.ShootCircleIntervals)
        {
            ShootCircleIntervals();
        }
        else if (attackingType == TypesOfAttack.ShootCircleRandIntervals)
        {
            ShootCircleRandIntervals();
        }
        else if (attackingType == TypesOfAttack.BasicShootRandIntervals)
        {
            BasicShootRandIntervals();
        }
    }

    void FixedUpdate()
    {
        if(projAmount != 0)
            circleAngle = 360 / projAmount;

        if (enemyMove)
        {
            if (enemyMove.InDistance(enemyMove.OverallRadiusFollow))
            {
                ChoseAttackType();
            }
        }
        else
        {
            ChoseAttackType();
        }

        for (int i = 0; i < colliding.Count; i++)
        {
            CheckCollisions(colliding[i]);
        }
    }

    public void Shootforward()
    {
        if (burstFire)
            StartCoroutine(BurstFire());
        else
            Shoot();
    }

    public void DoShoot()
    {
        //create the projecticle
        GameObject projectile = ObjectPooler.GetPooledObject(projecticle);

        if (!shootPosition)
            projectile.transform.position = transform.position;
        else
            projectile.transform.position = shootPosition.transform.position;

        projectile.transform.rotation = transform.rotation;

        projectile.GetComponent<ProjectileCollision>().damageMultiplyer = projectileDmgMutliplier;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * thrust, ForceMode.Impulse);
        projectile.GetComponent<ProjectileCollision>().thrust = thrust;

        //do sound
        SoundManager.PlaySound(shootSounds, transform.position);
    }

    void Shoot()
    {
        //create the projecticle
        GameObject projectile = ObjectPooler.GetPooledObject(projecticle);

        Physics.IgnoreCollision(col, projectile.GetComponent<Collider>(), true);

        if (!shootPosition)
            projectile.transform.position = transform.position;
        else
            projectile.transform.position = shootPosition.transform.position;

        if (!usesChildRotation)
            projectile.transform.rotation = transform.rotation;
        else
            projectile.transform.rotation = transform.GetChild(0).transform.rotation;

        projectile.GetComponent<ProjectileCollision>().damageMultiplyer = projectileDmgMutliplier;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * thrust, ForceMode.Impulse);
        projectile.GetComponent<ProjectileCollision>().thrust = thrust;

        //do sound
        SoundManager.PlaySound(shootSounds, transform.position);
    }

    IEnumerator BurstFire()
    {
        animator.enabled = false;

        for (int i = 0; i < burstAmount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        animator.enabled = true;
    }

    void BasicShootIntervals()
    {
        shootIntervalCounter++;
        //count up until the counter has reached the interval time, then shoot bullet and restart
        if (shootIntervalCounter > (timeTillInterval * 60))
        {
            animator.SetTrigger("Attack");
            
            shootIntervalCounter = 0;
        } 
    }

    void BasicShootRandIntervals()
    {
        shootIntervalCounter++;
        //count up until the counter has reached the interval time, then shoot bullet and restart
        if (shootIntervalCounter > (randomInterval * 60))
        {
            randomInterval = Random.Range(minInterval, maxInterval);

            animator.SetTrigger("Attack");

            shootIntervalCounter = 0;
        } 
    }

    public void ShootCircle()
    {
        angle = circleAngle;
        for (int i = 0; i < projAmount; i++)
        {
            //create the projecticle
            GameObject projectile = ObjectPooler.GetPooledObject(projecticle);

            Physics.IgnoreCollision(col, projectile.GetComponent<Collider>(), true);

            projectile.transform.rotation = transform.rotation;
            projectile.transform.Rotate(0, angle, 0);

            if (!shootPosition)
                projectile.transform.position = transform.position;
            else
                projectile.transform.position = shootPosition.transform.position;
            
            projectile.GetComponent<ProjectileCollision>().damageMultiplyer = projectileDmgMutliplier;
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * thrust, ForceMode.Impulse);
            projectile.GetComponent<ProjectileCollision>().thrust = thrust;
            angle += circleAngle;
        }

        //do sound
        SoundManager.PlaySound(shootSounds, transform.position);
    }

    public void SplitEnemy(GameObject prefab, int amount, bool addToGenerator)
    {
        for (int i = 0; i < amount; i++)
        {
            //create the split enemies and set them to this position
            GameObject enemy = ObjectPooler.GetPooledObject(prefab);

            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;

            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(new Vector3(transform.position.x, 0, transform.position.z));

            if (addToGenerator)
            {
                if (LevelGenerator.Instance)
                {
                    if (LevelGenerator.Instance.currentTile)
                    {
                        if (LevelGenerator.Instance.currentTile.GetComponentInChildren<EnemySpawner>())
                            LevelGenerator.Instance.currentTile.GetComponentInChildren<EnemySpawner>().spawnedEnemies.Add(enemy);     
                    }
                }
            }
        }
    }

    float GetGreaterNumber(float num1, float num2)
    {
        if (num1 > num2)
            return num1;
        else
            return num2;
    }

    float GetLessNumber(float num1, float num2)
    {
        if (num1 < num2)
            return num1;
        else
            return num2;
    }

    protected void ShootBetweenTwoAngles(float angle1, float angle2, int projAmount, bool usesRotation)
    {
        float transformAngle = 0;

        if (usesRotation)
        {
            if (usesChildRotation)
                transformAngle += transform.GetChild(0).transform.eulerAngles.y;
            else 
                transformAngle += transform.eulerAngles.y;
        }

        //get the total angle
        float totalAngle = GetGreaterNumber(angle1, angle2) - GetLessNumber(angle1, angle2);

        //get distance between each of the projectile
        float distanceBetweenEachProj = totalAngle / projAmount;

        float angle = GetLessNumber(angle1, angle2);

        for (int i = 0; i < projAmount; i++)
        {
            //create the projecticle
            GameObject projectile = ObjectPooler.GetPooledObject(projecticle);

            Physics.IgnoreCollision(col, projectile.GetComponent<Collider>(), true);

            projectile.transform.rotation = transform.rotation;

            projectile.transform.Rotate(0, angle, 0);

            if (!shootPosition)
                projectile.transform.position = transform.position;
            else
                projectile.transform.position = shootPosition.transform.position;

            projectile.GetComponent<ProjectileCollision>().damageMultiplyer = projectileDmgMutliplier;
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * thrust, ForceMode.Impulse);
            projectile.GetComponent<ProjectileCollision>().thrust = thrust;

            angle += distanceBetweenEachProj;
        }
    }

    void ShootCircleIntervals()
    {
        shootIntervalCounter++;
        //count up until the counter has reached the interval time, then shoot bullet and restart
        if (shootIntervalCounter > (timeTillInterval * 60))
        {
            animator.SetTrigger("Attack");
            
            shootIntervalCounter = 0;
        } 
    }

    void ShootCircleRandIntervals()
    {
        shootIntervalCounter++;
        //count up until the counter has reached the interval time, then shoot bullet and restart
        if (shootIntervalCounter > (randomInterval * 60))
        {
            randomInterval = Random.Range(minInterval, maxInterval);

            animator.SetTrigger("Attack");

            shootIntervalCounter = 0;
        } 
    }

    void CheckCollisions(Colliding c)
    {
        if (c.col.gameObject.GetComponent<PlayerInformation>())
        {
            PlayerInformation playerInfo = c.col.gameObject.GetComponent<PlayerInformation>();
            //checks to see if the player is invincible or not
            if (!c.col.gameObject.GetComponent<PlayerInformation>().invincible)
            {
                //gets the direction
                Vector3 dir = transform.position - c.col.transform.position;
                //add knockback to the player
                playerInfo.KnockbackPlayer(-dir, knockbackStrength);
                if (!c.col.transform.GetComponent<PlayerAttack>().blocking)
                {
                    c.health.Damaged();
                    c.health.AffectHealth(-damageOnTouch / playerInfo.resistance);
                }
                else
                {
                    //checks if the user is facing whatever the collision is coming from
                    float dot = Vector3.Dot(c.col.transform.forward, (transform.position - c.col.transform.position).normalized);
                    if (dot < 0.5f)
                    {
                        c.health.Damaged();
                        c.health.AffectHealth(-damageOnTouch / playerInfo.resistance);

                        //add knockback to the player
                        playerInfo.KnockbackPlayer(-dir, knockbackStrength);
                    }
                    else
                    {
                        //add knockback to the player
                        playerInfo.KnockbackPlayer(-dir, knockbackStrength / 2);
                        SoundManager.PlaySound(playerInfo.playerAttack.hitBlockSound, transform.position);
                    }
                }

                if (enemyMove)
                    enemyMove.runAwayForSeconds();
            }
        }
        else
        {
            if (damagesOtherEnemies)
            {
                if (floorSpikes)
                {
                    if (c.col.tag != "Flying")
                        c.health.AffectHealth(-damageOnTouch);
                }
            } 
        }

        if (enemyHealth)
        {
            //if slowly dying, set whatever touches it to slowly dying
            if (enemyHealth.isSlowlyDying)
            {
                if (!c.health.isSlowlyDying)
                    c.health.SetSlowDeath();
            }
        }

        colliding.Clear();
    }

    void OnCollisionEnter(Collision col)
    {
        Health health = col.transform.GetComponent<Health>();

        Colliding c;

        c.health = health;
        c.col = col.collider;

        if (health)
        {
            if (!colliding.Contains(c))
            {
                colliding.Add(c);
                CheckCollisions(c);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Health health = col.transform.GetComponent<Health>();

        Colliding c;

        c.health = health;
        c.col = col;

        if (health)
        {
            if (!colliding.Contains(c))
            {
                colliding.Add(c);
                CheckCollisions(c);
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        Health health = col.transform.GetComponent<Health>();

        Colliding c;

        c.health = health;
        c.col = col.collider;

        if (health)
        {
            if (!colliding.Contains(c))
                colliding.Remove(c);
        }
    }

    void OnTriggerExit(Collider col)
    {
        Health health = col.transform.GetComponent<Health>();

        Colliding c;

        c.health = health;
        c.col = col;

        if (health)
        {
            if (!colliding.Contains(c))
                colliding.Remove(c);
        }
    }

    public void SpikeSound()
    {
        SoundManager.PlaySound(SoundManager.Instance.spikeSound, transform.position);

        FloorSpikes floorSpike = GetComponentInParent<FloorSpikes>();

        SpawnEffects.EffectOnHit(floorSpike.particleOnUp, transform.position);
    }
}
