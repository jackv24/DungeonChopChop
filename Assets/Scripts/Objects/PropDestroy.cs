using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDestroy : MonoBehaviour
{

    [Header("Attacks that destroy this prop")]
    public bool slashDestroysIt = true;
    public bool tripleDestroysIt = true;
    public bool dashDestroysIt = true;
    public bool spinDestroysIt = true;

    [Header("Particles")]
    [Tooltip("Amount of different particle types eg 'Dust, Smoke, Shrapnel'")]
    public AmountOfParticleTypes[] amountOfParticleTypes;

    [Header("Sounds")]
    public SoundEffect hitSounds;
    public SoundEffect destroySounds;

    [Space()]
    public CameraShakeVars destroyShake;

    private Health propHealth;

    // Use this for initialization
    void Start()
    {
        propHealth = GetComponent<Health>();
        if (propHealth)
            propHealth.OnHealthChange += UpdateProp;
    }

    // Update is called once per frame
    void UpdateProp()
    {
        if (propHealth.health <= 0)
        {
            //do effects
            SpawnEffects.EffectOnDeath(amountOfParticleTypes, transform.position);

            //do sound
            SoundManager.PlaySound(destroySounds, transform.position);
            //do drop
            GetComponent<Drops>().DoDrop();

            CameraShake.ShakeScreen(destroyShake.magnitude, destroyShake.shakeAmount, destroyShake.duration);

            Destroy(gameObject);
        }
    }

    public void DoEffect()
    {
        SpawnEffects.EffectOnHit(amountOfParticleTypes, new Vector3(transform.position.x, GetComponent<Collider>().bounds.max.y + .1f, transform.position.z));
    }

    public void HitSound()
    {
        SoundManager.PlaySound(hitSounds, transform.position);
    }

    void DoHitEffectAndSound()
    {
        //do the props effect then destroy it
        if (GetComponent<Collider>().GetComponent<BoxCollider>().enabled)
        {
            DoEffect();
            HitSound();
        }
    }

    void DestroyProps(Collider collider)
    {
        //if its the sword colliding
        if (collider.gameObject.layer == 16)
        {
            Animator anim = collider.gameObject.GetComponentInParent<Animator>();
            PlayerInformation playerInfo = collider.gameObject.GetComponentInParent<PlayerInformation>();

            if (propHealth && anim && playerInfo)
            {
                if (collider.gameObject.GetComponentInParent<Animator>())
                {
                    //check if in slashing state
                    if (anim.GetCurrentAnimatorStateInfo(1).IsTag("Attacking") || anim.GetCurrentAnimatorStateInfo(1).IsTag("SecondAttack"))
                    {
                        if (slashDestroysIt)
                        {
                            propHealth.AffectHealth(-playerInfo.GetSwordDamage());
                            DoHitEffectAndSound();
                        }
                    }
                    //check if in dashing
                    else if (anim.GetCurrentAnimatorStateInfo(0).IsTag("DashAttack"))
                    {
                        if (dashDestroysIt)
                        {
                            propHealth.AffectHealth(-playerInfo.GetSwordDamage());
                            DoHitEffectAndSound();
                        }
                    }
                    //check if in Spinning
                    else if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
                    {
                        if (spinDestroysIt)
                        {
                            propHealth.AffectHealth(-playerInfo.GetSwordDamage());
                            DoHitEffectAndSound();
                        }

                    }
                    //check if in Spinning
                    else if (anim.GetCurrentAnimatorStateInfo(1).IsTag("RapidAttack"))
                    {
                        if (tripleDestroysIt)
                        {
                            propHealth.AffectHealth(-playerInfo.GetSwordDamage());
                            DoHitEffectAndSound();
                        }
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        DestroyProps(collision.collider);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (GetComponent<Collider>().isTrigger)
            DestroyProps(collider);
    }
}
