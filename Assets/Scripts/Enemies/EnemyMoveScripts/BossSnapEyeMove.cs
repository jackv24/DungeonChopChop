using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSnapEyeMove : EnemyMove {
    
    public int enemiesToSpawn = 10;
    public GameObject enemy;

    private bool Attacking = false;
    private List<GameObject> enemies = new List<GameObject>();

    // Use this for initialization
    void Start () 
    {
        Setup(); 
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update () 
    {
        if (!Attacking)
        {
            if (EnemiesDead())
            {
                StartAttacking();
            }
        }
        else
        {
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
