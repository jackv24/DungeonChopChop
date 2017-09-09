using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour {

    public GameObject trail;
    public GameObject[] hitParticles;

    [Header("Camera Shake Values")]
    public float magnitude = 1;
    public float shakeAmount = 1;
    public float duration = 1;
    public float pauseTime = .1f;

    [HideInInspector()]
    public PlayerAttack playerAttack;
    [HideInInspector()]
    public PlayerInformation playerInfo;
    [HideInInspector()]
    public Health playerHealth;
    [HideInInspector()]
    public Animator animator;

    private Collider col;
    private SwordStats swordStats;

	// Use this for initialization
	void Start () {
        playerInfo = GetComponentInParent<PlayerInformation>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerHealth = GetComponentInParent<Health>();
        animator = GetComponentInParent<Animator>();
        col = GetComponent<Collider>();
        swordStats = GetComponent<SwordStats>();
	}

    void DoParticle(Collision col)
    {
        int random = Random.Range(0, hitParticles.Length);
        GameObject particle = ObjectPooler.GetPooledObject(hitParticles[random]);
        particle.transform.position = col.contacts[0].point;
    }

    void OnCollisionEnter(Collision col)
    {
        //check if the collider is on the enemy layer
        if (col.gameObject.layer == 11)
        {
            if (col.gameObject.GetComponent<Health>())
            {
                Health enemyHealth = col.gameObject.GetComponent<Health>();
                //calculates knockback depending on direction
                enemyHealth.Knockback(playerInfo, playerAttack.transform.forward);
                //checks if the player has a status condition
                DoParticle(col);
                if (playerHealth.HasStatusCondition())
                {
                    //if the player is burned or poisoned, a charm may affect the damage output
                    if (playerHealth.isBurned || playerHealth.isPoisoned)
                    {
                        enemyHealth.AffectHealth((-playerInfo.strength * playerAttack.sword.damageMultiplier * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit()) * playerInfo.GetCharmFloat("dmgMultiWhenBurned") * playerInfo.GetCharmFloat("dmgMultiWhenPoisoned"));
                    }
                    else
                    {
                        enemyHealth.AffectHealth(-playerInfo.strength * playerAttack.sword.damageMultiplier * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit());
                    }
                }
                else
                {
                    //else just do the normal damage
                    enemyHealth.AffectHealth(-playerInfo.strength * playerAttack.sword.damageMultiplier * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit());
                }
                //checks what state the players animation is in
                if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking") || animator.GetCurrentAnimatorStateInfo(1).IsTag("TripleAttack"))
                {
                    if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < .7f)
                    {
                        //do the little pause for feelings
                        StartCoroutine(QuickGamePause(col.collider));
                    }
                }
                if (swordStats.weaponEffect == WeaponEffect.Burn)
                {
                    enemyHealth.SetBurned(swordStats.damagePerTick, swordStats.duration, swordStats.timeBetweenEffect);
                }
                else if (swordStats.weaponEffect == WeaponEffect.Poison)
                {
                    enemyHealth.SetPoison(swordStats.damagePerTick, swordStats.duration, swordStats.timeBetweenEffect);
                }
                else if (swordStats.weaponEffect == WeaponEffect.SlowDeath)
                {
                    enemyHealth.SetSlowDeath(swordStats.damagePerTick, swordStats.duration, swordStats.timeBetweenEffect);
                }
                CameraShake.ShakeScreen(magnitude, shakeAmount, duration);
            }
        }
    }

    IEnumerator QuickGamePause(Collider col)
    {
        animator.enabled = false;
        col.GetComponentInChildren<Animator>().enabled = false;
        yield return new WaitForSecondsRealtime(pauseTime);
        col.GetComponentInChildren<Animator>().enabled = true;
        animator.enabled = true;
    }

}
