using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour {


    public float knockbackOnHit = 5;
    public GameObject trail;
    public GameObject[] hitSmokes;
    public GameObject[] hitEffects;

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

    private SwordStats swordStats;

	// Use this for initialization
	void Start () {
        playerInfo = GetComponentInParent<PlayerInformation>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerHealth = GetComponentInParent<Health>();
        animator = GetComponentInParent<Animator>();
        swordStats = GetComponent<SwordStats>();
	}

    void DoParticle(Collision col)
    {
        int randomSmoke = Random.Range(0, hitSmokes.Length);
        int randomEffect = Random.Range(0, hitEffects.Length);
        GameObject smoke = ObjectPooler.GetPooledObject(hitSmokes[randomSmoke]);
        smoke.transform.position = new Vector3(col.contacts[0].point.x, col.collider.bounds.max.y + .1f, col.contacts[0].point.z);
        GameObject effect = ObjectPooler.GetPooledObject(hitEffects[randomEffect]);
        effect.transform.position = new Vector3(col.contacts[0].point.x, col.collider.bounds.max.y + .1f, col.contacts[0].point.z);
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

                DoParticle(col);

                //else just do the normal damage
                enemyHealth.AffectHealth(-playerInfo.GetSwordDamage());

                //checks what state the players animation is in
                if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking") || animator.GetCurrentAnimatorStateInfo(1).IsTag("TripleAttack"))
                {
                    if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < .7f)
                    {
                        //do the little pause for feelings
                        //StartCoroutine(QuickGamePause(col.collider));
                    }
                }
                //set the status effect depending on the weapons effect
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

                playerInfo.KnockbackPlayer(-playerInfo.transform.forward, knockbackOnHit);
            }
        }
        //if the player == Prop
        else if (col.gameObject.layer == 17)
        {
            //do the props effect then destroy it
            col.gameObject.GetComponent<PropDestroy>().DoEffect();
            col.gameObject.GetComponent<PropDestroy>().hitAmount--;
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
