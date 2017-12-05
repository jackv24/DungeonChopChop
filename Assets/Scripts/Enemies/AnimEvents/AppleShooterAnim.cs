using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShooterAnim : MonoBehaviour {
    
    EnemyAttack enemyAttack;

    // Use this for initialization
    void Start () {
        enemyAttack = GetComponentInParent<EnemyAttack>();
    }

    public void Shoot()
    {
        enemyAttack.usesChildRotation = false;
        enemyAttack.Shootforward();
    }
}
