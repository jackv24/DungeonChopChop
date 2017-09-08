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

	// Use this for initialization
	void Start () {
        playerInfo = GetComponentInParent<PlayerInformation>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerHealth = GetComponentInParent<Health>();
        animator = GetComponentInParent<Animator>();
        col = GetComponent<Collider>();
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
                //calculates knockback depending on direction
                col.gameObject.GetComponent<Health>().Knockback(playerInfo, playerAttack.transform.forward);
                //checks if the player has a status condition
                DoParticle(col);
                if (playerHealth.HasStatusCondition())
                {
                    //if the player is burned or poisoned, a charm may affect the damage output
                    if (playerHealth.isBurned || playerHealth.isPoisoned)
                    {
                        col.gameObject.GetComponent<Health>().AffectHealth((-playerInfo.strength * playerAttack.sword.damageMultiplier * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit()) * playerInfo.GetCharmFloat("dmgMultiWhenBurned") * playerInfo.GetCharmFloat("dmgMultiWhenPoisoned"));
                    }
                    else
                    {
                        col.gameObject.GetComponent<Health>().AffectHealth(-playerInfo.strength * playerAttack.sword.damageMultiplier * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit());
                    }
                }
                else
                {
                    col.gameObject.GetComponent<Health>().AffectHealth(-playerInfo.strength * playerAttack.sword.damageMultiplier * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit());
                }
                if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking") || animator.GetCurrentAnimatorStateInfo(1).IsTag("TripleAttack"))
                {
                    if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < .7f)
                    {
                        StartCoroutine(QuickGamePause(col.collider));
                    }
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
