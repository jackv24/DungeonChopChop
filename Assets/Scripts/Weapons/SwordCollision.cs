using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour {


    public float knockbackOnHit = 5;
    public GameObject trail;
    public GameObject[] hitSmokes;
    public GameObject[] hitEffects;
    public GameObject spinReadyParticle;
    public GameObject chargeParticle;

    [HideInInspector()]
    public PlayerAttack playerAttack;
    [HideInInspector()]
    public PlayerInformation playerInfo;
    [HideInInspector()]
    public Health playerHealth;
    [HideInInspector()]
    public Animator animator;

    private SwordStats swordStats;
    [HideInInspector]
    public Coroutine chargeCoroutine;

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

    void SetEffect(Health health)
    {
        //set the status effect depending on the weapons effect
        if (swordStats.weaponEffect == WeaponEffect.Burn)
            health.SetBurned(swordStats.damagePerTick, swordStats.duration, swordStats.timeBetweenEffect);
        else if (swordStats.weaponEffect == WeaponEffect.Poison)
            health.SetPoison(swordStats.damagePerTick, swordStats.duration, swordStats.timeBetweenEffect);
        else if (swordStats.weaponEffect == WeaponEffect.SlowDeath)
            health.SetSlowDeath(swordStats.damagePerTick, swordStats.duration, swordStats.timeBetweenEffect);
        else if (swordStats.weaponEffect == WeaponEffect.Ice)
            health.SetIce(swordStats.duration);
        else if (swordStats.weaponEffect == WeaponEffect.Sandy)
            health.SetSandy(swordStats.duration, swordStats.speedDamper);
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

                SetEffect(enemyHealth);

                CameraShake.ShakeScreen(enemyHealth.hitShake.magnitude, enemyHealth.hitShake.shakeAmount, enemyHealth.hitShake.duration);

                playerInfo.KnockbackPlayer(-playerInfo.transform.forward, knockbackOnHit);
            }
        } 
        else if (col.gameObject.layer == 17)
        {
            if (swordStats.weaponEffect == WeaponEffect.Burn)
                col.gameObject.GetComponent<Health>().SetBurned(swordStats.damagePerTick, swordStats.duration, swordStats.timeBetweenEffect);
        }
    }

    ///////this function is in case we want the game to stutter when hitting an enemy
//    IEnumerator QuickGamePause(Collider col)
//    {
//        animator.enabled = false;
//        col.GetComponentInChildren<Animator>().enabled = false;
//        yield return new WaitForSecondsRealtime(pauseTime);
//        col.GetComponentInChildren<Animator>().enabled = true;
//        animator.enabled = true;
//    }

    public void DoChargeParticle()
    {
        if (chargeCoroutine == null)
        {
            chargeCoroutine = StartCoroutine(ChargeParticle());
        }
    }

    IEnumerator ChargeParticle()
    {
        yield return new WaitForSeconds(.2f);
        if (animator.GetCurrentAnimatorStateInfo(1).IsTag("SpinCharge"))
        {
            GameObject particle = ObjectPooler.GetPooledObject(chargeParticle);
            MeshFilter mesh = GetComponent<MeshFilter>();
            //set the parent to the sword
            particle.transform.parent = transform;
            //sets the position to the hielt of the sword
            particle.transform.localPosition = new Vector3(mesh.sharedMesh.bounds.center.x, mesh.sharedMesh.bounds.min.y, mesh.sharedMesh.bounds.center.z);
            //while the particle is not at the tip of the sword
            while (!playerAttack.spinChargeReady)
            {
                //stop if in idle state
                if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Idle"))
                {
                    particle.SetActive(false);
                    chargeCoroutine = null;
                    yield break;
                }
                //move towards the tip
                particle.transform.localPosition = Vector3.Lerp(particle.transform.localPosition, new Vector3(mesh.sharedMesh.bounds.center.x, mesh.sharedMesh.bounds.max.y, mesh.sharedMesh.bounds.center.z), 1.05f * Time.deltaTime);
                //stop if charge ready
                yield return new WaitForEndOfFrame();
            }
            particle.GetComponent<ParticleSystem>().Stop();

            //set player white
            playerHealth.SetWhite(playerAttack.whiteVal);

            //do sound
            SoundManager.PlaySound(playerAttack.chargeReadySounds, transform.position);

            //create the particle that shows when the spin attack is ready
            GameObject readyParticle = ObjectPooler.GetPooledObject(spinReadyParticle);
            readyParticle.transform.parent = transform;
            readyParticle.transform.localPosition = new Vector3(mesh.sharedMesh.bounds.center.x, mesh.sharedMesh.bounds.max.y, mesh.sharedMesh.bounds.center.z);

            yield return new WaitForSeconds(playerAttack.flashDuration);

            //set player normal color
            playerHealth.UnfadeWhite();

            particle.SetActive(false);

            while (animator.GetCurrentAnimatorStateInfo(1).IsTag("SpinCharge") || animator.GetCurrentAnimatorStateInfo(1).IsTag("Blocking"))
            {
                yield return new WaitForEndOfFrame();
            }

            readyParticle.GetComponent<ParticleSystem>().Stop();

            yield return new WaitForSeconds(.5f);
            readyParticle.SetActive(false);
            chargeCoroutine = null;
        }
        else
        {
            chargeCoroutine = null;
        }
    }
}
