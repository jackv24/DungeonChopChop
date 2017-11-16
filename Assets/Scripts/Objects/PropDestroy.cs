using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDestroy : MonoBehaviour
{

    [Header("Attacks that destroy this prop")]
    public bool slashDestroysIt = true;
    public bool tripleDestroysIt = true;
    public bool dashDestroysIt = true;
    public bool dashAttackDestroysIt = true;
    public bool spinDestroysIt = true;
    [Space()]
    public bool destroyedByTalons = false;

    [Header("Particles")]
    [Tooltip("Amount of different particle types eg 'Dust, Smoke, Shrapnel'")]
    public AmountOfParticleTypes[] amountOfParticleTypes;
    public AmountOfParticleTypes[] indistructableParticles;

    [Header("Sounds")]
    public SoundEffect hitSounds;
    public SoundEffect destroySounds;
    public SoundEffect indistructableSound;

    [Space()]
    public CameraShakeVars destroyShake;

    [System.Serializable]
    public class ReactAnimation
    {
        public bool animate = false;
        public AnimationCurve curve;
        public float duration;

        private Quaternion oldRotation;

        private Coroutine routine;

        public void Play(MonoBehaviour owner, GameObject other)
        {
            if(!animate)
                return;

            if (routine != null)
            {
                owner.StopCoroutine(routine);
                owner.transform.rotation = oldRotation;
            }

            oldRotation = owner.transform.rotation;

            routine = owner.StartCoroutine(PlayRoutine(owner.gameObject, other));
        }

        IEnumerator PlayRoutine(GameObject owner, GameObject other)
        {
            Vector3 offset = owner.transform.position - other.transform.position;
            offset.y = 0;
            offset.Normalize();

            Vector3 right = Vector3.Cross(offset, Vector3.up);

            Quaternion initialRotation = owner.transform.rotation;

            float elapsed = 0;

            while(elapsed < duration)
            {
                owner.transform.rotation = initialRotation;

                float angle = curve.Evaluate(elapsed / duration);
                owner.transform.Rotate(right, angle);

                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }
        }
    }
    [Header("Animation")]
    public ReactAnimation hitAnim;

    private Health propHealth;
    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }

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

            if (GetComponent<Drops>())
                GetComponent<Drops>().DoDrop();

            CameraShake.ShakeScreen(destroyShake.magnitude, destroyShake.shakeAmount, destroyShake.duration);

            Destroy(gameObject);
        }
    }

    public void DoEffect()
    {
        SpawnEffects.EffectOnHit(amountOfParticleTypes, new Vector3(transform.position.x, GetComponent<Collider>().bounds.max.y + .1f, transform.position.z));
    }

    void DoIndistructableEffect()
    {
        if (GetComponent<Collider>().GetComponent<BoxCollider>().enabled)
        {
            SpawnEffects.EffectOnHit(indistructableParticles, new Vector3(transform.position.x, GetComponent<Collider>().bounds.max.y + .1f, transform.position.z));
            HitSound(indistructableSound);
        }
    }

    public void HitSound(SoundEffect sound)
    {
        SoundManager.PlaySound(sound, transform.position);
    }

    void DoHitEffectAndSound()
    {
        //do the props effect then destroy it
        if (GetComponent<Collider>().GetComponent<BoxCollider>().enabled)
        {
            DoEffect();
            HitSound(hitSounds);
        }
    }

    void DestroyProps(Collider collider)
    {
        //if its the sword colliding
        if (collider.gameObject.layer == 16)
        {
            hitAnim.Play(this, collider.gameObject);

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
                            DamageText.Show(-playerInfo.GetSwordDamage(), col.bounds.center);
                        }
                        else
                            DoIndistructableEffect();
                    }
                    //check if in dashing
                    else if (anim.GetCurrentAnimatorStateInfo(0).IsTag("DashAttack"))
                    {
                        if (dashDestroysIt)
                        {
                            propHealth.AffectHealth(-playerInfo.GetSwordDamage());
                            DoHitEffectAndSound();
                            DamageText.Show(-playerInfo.GetSwordDamage(), col.bounds.center);
                        }
                        else
                            DoIndistructableEffect();
                    }
                    //check if in Spinning
                    else if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
                    {
                        if (spinDestroysIt)
                        {
                            propHealth.AffectHealth(-playerInfo.GetSwordDamage());
                            DoHitEffectAndSound();
                            DamageText.Show(-playerInfo.GetSwordDamage(), col.bounds.center);
                        }
                        else
                            DoIndistructableEffect();
                    }
                    //check if in Spinning
                    else if (anim.GetCurrentAnimatorStateInfo(1).IsTag("RapidAttack"))
                    {
                        if (tripleDestroysIt)
                        {
                            propHealth.AffectHealth(-playerInfo.GetSwordDamage());
                            DoHitEffectAndSound();
                            DamageText.Show(-playerInfo.GetSwordDamage(), col.bounds.center);
                        }
                        else
                            DoIndistructableEffect();
                    }
                }
            }
        }

        //the player layer
        if (collider.gameObject.layer == 14)
        {
            if (destroyedByTalons)
            {
                if (ItemsManager.Instance.hasBoots)
                {
                    propHealth.AffectHealth(-10);
                    DoHitEffectAndSound();
                }
            }

            else if (dashDestroysIt)
            {
                if (collider.gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(1).IsTag("Dash"))
                {
                    propHealth.AffectHealth(-10);
                    DoHitEffectAndSound();
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
