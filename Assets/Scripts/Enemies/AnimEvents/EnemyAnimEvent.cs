﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour {

    EnemyAttack enemyAttack;

    // Use this for initialization
    void Start () {
        enemyAttack = GetComponentInParent<EnemyAttack>();
    }

    public void Shoot()
    {
        enemyAttack.usesChildRotation = false;
        enemyAttack.DoShoot();
    }

    public void ShootCircle()
    {
        enemyAttack.usesChildRotation = false;
        enemyAttack.ShootCircle();
    }
}
