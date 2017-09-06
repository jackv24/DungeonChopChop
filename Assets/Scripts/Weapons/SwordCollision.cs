using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour {

    public GameObject particle;
    public GameObject trail;

    [Header("Camera Shake Values")]
    public float magnitude = 1;
    public float shakeAmount = 1;
    public float duration = 1;
    public float pauseTime = .1f;

    private PlayerAttack playerAttack;
    private PlayerInformation playerInfo;
    private Health playerHealth;
    private Animator animator;
    private Collider col;

	// Use this for initialization
	void Start () {
        playerInfo = GetComponentInParent<PlayerInformation>();
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerHealth = GetComponentInParent<Health>();
        animator = GetComponentInParent<Animator>();
        col = GetComponent<Collider>();
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
                GameObject p = Instantiate(particle, col.contacts[0].point, Quaternion.Euler(0, 0, 0)) as GameObject;
                Destroy(p, .2f);
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
                        StartCoroutine(QuickGamePause());
                    }
                }
                CameraShake.ShakeScreen(magnitude, shakeAmount, duration);
            }
        }
    }

    IEnumerator QuickGamePause()
    {
        Time.timeScale = .1f;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }

}
