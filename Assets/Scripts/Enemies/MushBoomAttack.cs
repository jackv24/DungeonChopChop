using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoomAttack : EnemyAttack {

    [Header("Mush Values")]
    [Tooltip("Hit == Boom Particles")]
    public AmountOfParticleTypes[] boomParticles;

    public void Boom()
    {
        SpawnEffects.EffectOnHit(boomParticles, transform.position);
        enemyDeath.StatusExplode();
    }
}
