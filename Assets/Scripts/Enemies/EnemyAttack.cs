using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfAttack
{
    Nothing,
    BasicShootIntervals,
    BasicShootRandIntervals,
    ShootCircleIntervals,
    ShootCircleRandIntervals,
};

public class EnemyAttack : MonoBehaviour
{

    public TypesOfAttack attackingType;

    [Header("Projectile Vars")]
    [HideInInspector]
    public GameObject projecticle;
    [HideInInspector]
    public GameObject shootPosition;
    [HideInInspector]
    public float thrust;

    [Space()]
    [Tooltip("This values x projectile damage")]
    public float attackStrength;
    public float damageOnTouch;
    public float knockbackStrength;

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

    private float shootIntervalCounter = 0;
    private int circleAngle = 0;
    private float angle = 0;
    private float randomInterval = 0;

    protected EnemyMove enemyMove;
    protected Animator animator;
    protected Health enemyHealth;

    LevelTile parentTile = null;

    void Start()
    {
        enemyHealth = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
        enemyMove = GetComponent<EnemyMove>();

        //If this enemy was spawned as a child of a tile, only enable when tile is entered
        parentTile = GetComponentInParent<LevelTile>();

        if(parentTile)
        {
            parentTile.OnTileEnter += SetActive;
            parentTile.OnTileExit += SetActive;

            SetInactive();
        }
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

    void FixedUpdate()
    {
        if(projAmount != 0)
            circleAngle = 360 / projAmount;

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

    public void Shootforward()
    {
        //create the projecticle
        GameObject projectile = ObjectPooler.GetPooledObject(projecticle);
        projectile.transform.position = shootPosition.transform.position;
        projectile.transform.rotation = transform.rotation;
        projectile.GetComponent<ProjectileCollision>().damageMultiplyer = attackStrength;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * thrust, ForceMode.Impulse);
        projectile.GetComponent<ProjectileCollision>().thrust = thrust;
    }

    void BasicShootIntervals()
    {
        shootIntervalCounter++;
        //count up until the counter has reached the interval time, then shoot bullet and restart
        if (shootIntervalCounter > (timeTillInterval * 60))
        {
            animator.SetTrigger("Attack");
            //Shootforward();
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
            //Shootforward();
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
            if (!shootPosition)
                projectile.transform.position = transform.position;
            projectile.transform.rotation = transform.rotation;
            projectile.transform.Rotate(0, angle, 0);
            projectile.GetComponent<ProjectileCollision>().damageMultiplyer = attackStrength;
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * thrust, ForceMode.Impulse);
            projectile.GetComponent<ProjectileCollision>().thrust = thrust;
            angle += circleAngle;
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
            ShootCircle();
            shootIntervalCounter = 0;
        } 
    }

    void CheckCollisions(Collider col)
    {
        if (col.transform.GetComponent<Health>())
        {
            if (col.gameObject.GetComponent<PlayerInformation>())
            {
                PlayerInformation playerInfo = col.gameObject.GetComponent<PlayerInformation>();
                //checks to see if the player is invincible or not
                if (!col.gameObject.GetComponent<PlayerInformation>().invincible)
                {
                    //gets the direction
                    Vector3 dir = transform.position - col.transform.position;
                    //add knockback to the player
                    playerInfo.KnockbackPlayer(-dir, knockbackStrength);
                    if (!col.transform.GetComponent<PlayerAttack>().blocking)
                    {
                        col.transform.GetComponent<Health>().Damaged();
                        col.transform.GetComponent<Health>().AffectHealth(-damageOnTouch / playerInfo.resistance);
                    }
                    else
                    {
                        if (col.transform.GetComponent<PlayerAttack>().shield.blockingResistance > 0)
                        {
                            col.transform.GetComponent<Health>().Damaged();
                            col.transform.GetComponent<Health>().AffectHealth(-damageOnTouch / playerInfo.resistance * col.transform.GetComponent<PlayerAttack>().shield.blockingResistance);
                        }
                    }

                    if (enemyMove)
                        enemyMove.runAwayForSeconds();
                }
            }
        }
    }

    void OnCollisionStay(Collision col)
    {
        CheckCollisions(col.collider);
    }
    void OnTriggerStay(Collider col)
    {
        CheckCollisions(col);
    }

}
