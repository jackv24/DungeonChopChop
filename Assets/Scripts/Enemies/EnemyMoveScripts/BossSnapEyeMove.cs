using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSnapEyeMove : EnemyMove {

    public bool spawnEnemies = false;
    public int enemiesToSpawn = 10;
    public GameObject enemy;

    private bool Attacking = false;
    private Collider[] enemyColliders = new Collider[0];
    private List<GameObject> enemies = new List<GameObject>();
    private Health enemyHealth;

    // Use this for initialization
    void Start () 
    {
        Setup();
        enemyHealth = GetComponent<Health>();
        if (spawnEnemies)
            SpawnEnemies();
    }

    void OnEnable()
    {
        base.OnEnable();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Update is called once per frame
    void Update () 
    {
        if (enemyColliders.Length == 0 || enemyColliders == null)
        {
            GetEnemies();
        }

        if (!Attacking)
        {
            enemyHealth.enabled = false;
            if (spawnEnemies)
            {
                if (EnemiesDead())
                {
                    StartAttacking();
                }
            }
            else
            {
                if (EnemiesDeadCol())
                {
                    StartAttacking();
                }
            }
        }
        else
        {
            enemyHealth.enabled = true;
            FollowPlayer();
        }
    }

    bool EnemiesDead()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    bool EnemiesDeadCol()
    {
        foreach (Collider enemy in enemyColliders)
        {
            if (enemy != GetComponent<Collider>())
            {
                if (enemy.gameObject.activeSelf)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void GetEnemies()
    {
        enemyColliders = Physics.OverlapSphere(transform.position, 100, layerMask);
    }
        
    void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject newEnemy = ObjectPooler.GetPooledObject(enemy);
            enemy.transform.position = transform.position - transform.forward;
            enemies.Add(newEnemy);
        }
    }

    void StartAttacking()
    {
        animator.SetTrigger("Triggered");
        Attacking = true;
    }
}
