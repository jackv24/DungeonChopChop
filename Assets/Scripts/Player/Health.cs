using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float health;
    [Tooltip("The heavier the weight, less knockback")]
    public float weight;

    public delegate void HealthEvent();

    public event HealthEvent OnDeath;
    public event HealthEvent OnHealthChange;

    [Space()]
    public bool IsEnemy;
    public bool isProp;

    [Space()]
    public bool isPoisoned = false;
    public bool isBurned = false;
    public bool isSlowlyDying = false;
    public bool isFrozen = false;
    public bool isSandy = false;

    [Space()]
    public bool isDead = false;

    [Header("Tick Colors")]
    public Color poisonColor;
    public Color burnColor;
    public Color slowlyDyingColor;
    public Color frozenColor;
    public Color sandyColor;

    [Header("Hit Particles and Sounds")]
    public AmountOfParticleTypes[] hitParticles;
    public SoundEffect hitSounds;
    public Color hitColor;

    [Header("Ailment Tick Sounds")]
    public SoundEffect poisonTickSound;
    public SoundEffect burnTickSound;
    public SoundEffect slowDeathTickSound;
    [Header("Ailment start sound")]
    public SoundEffect poisonedSound;
    public SoundEffect burnSound;
    public SoundEffect sandySound;
    public SoundEffect slowDeathSound;
    public SoundEffect frozenSound;
    [Header("Ailment end sound")]
    public SoundEffect unpoisonedSound;
    public SoundEffect unburnSound;
    public SoundEffect unsandySound;
    public SoundEffect unslowDeathSound;
    public SoundEffect unfrozenSound;

    [Space()]
    [Header("Other Vals")]
    public float timeBetweenFlash = 0.1f;
    public int amountToFlash = 5;
    public float fadeToColorTime = 5;

    [Header("Camera Shake Values")]
    public float magnitude = 1;
    public float shakeAmount = 1;
    public float duration = 1;
    public float pauseTime = .1f;

    private PlayerInformation playerInfo;
    private Animator animator;
    private Rigidbody rb;

    private Renderer[] renderers;
    private List<Color> originalColors = new List<Color>();
    private Vector3 targetPosition;

    private Coroutine coroutine;

    private bool fadeToColor = false;
    private bool fadeToWhite = false;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        //loops through and get the original color on each renderer
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].sharedMaterial.HasProperty("_Color"))
                originalColors.Add(renderers[i].sharedMaterial.color);
        }

        rb = GetComponent<Rigidbody>();

        if (GetComponentInChildren<Animator>())
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (GetComponent<PlayerInformation>())
        {
            playerInfo = GetComponent<PlayerInformation>();
        }
    }

    public void AffectHealth(float healthDeta)
    {
        //props don't need to do this
        if (!isProp)
        {
            if (health > 0)
            {
                TemporaryInvincibility();
            }
        }
        
        health += healthDeta;
        if (OnHealthChange != null)
        {
            OnHealthChange();
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0 && isDead == false)
        {
            isDead = true;
            if (OnDeath != null)
            {
                OnDeath();
            }
        }
        if (IsEnemy)
        {
            if (!HasStatusCondition())
            {
                DoHitParticle();
                DoHitSound();
                HitColorFlash();
            }
        }
        else if (!IsEnemy)
        {
            if (animator)
                animator.SetTrigger("Hit");
            DoHitParticle();
            DoHitSound();
        }
    }

    void DoHitSound()
    {
        SoundManager.PlaySound(hitSounds, transform.position);
    }

    void DoHitParticle()
    {
        SpawnEffects.EffectOnHit(hitParticles, transform.position);
    }

    void OnEnable()
    {
        if (IsEnemy)
        {
            //sets the color of the enemies back to normal
            SetOGColor();
        }
        if (HasStatusCondition())
        {
            isPoisoned = false;
            isBurned = false;
            isSlowlyDying = false;
            isFrozen = false;
            isSandy = false;
        }
        health = maxHealth;
        isDead = false;
    }

    public void TemporaryInvincibility()
    {
        if (playerInfo)
        {
            if (!HasStatusCondition())
            {
                StartCoroutine(InvincibilityWait(playerInfo));
            }
        }
    }

    public void HealthChanged()
    {
        OnHealthChange();
    }

    IEnumerator InvincibilityWait(PlayerInformation playerInfo)
    {
        playerInfo.invincible = true;
        HitFlash();
        yield return new WaitForSeconds(playerInfo.invincibilityTimeAfterHit);
        playerInfo.invincible = false;
    }

    public void InvincibilityForSecs(float seconds)
    {
        StartCoroutine(InvincibilityForSeconds(seconds));
    }

    IEnumerator InvincibilityForSeconds(float seconds)
    {
        playerInfo.invincible = true;
        yield return new WaitForSeconds(seconds);
        playerInfo.invincible = false;
    }

    public void Knockback(PlayerInformation playerInfo, Vector3 direction)
    {
        if (rb)
        {
            StartCoroutine(DisableNav(1));
            rb.AddForce(direction * (playerInfo.knockback / weight) * playerInfo.GetCharmFloat("knockbackMultiplier"), ForceMode.Impulse);
        }
    }

    public void Knockback2(float knockback, Vector3 direction)
    {
        if (rb)
        {
            StartCoroutine(DisableNav(1));
            rb.AddForce(direction * (knockback / weight), ForceMode.Impulse);
        }
    }

    IEnumerator DisableNav(float seconds)
    {
        if (GetComponent<UnityEngine.AI.NavMeshAgent>())
        {
            GetComponent<EnemyMove>().usingNav = false;
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            yield return new WaitForSeconds(seconds);
            GetComponent<EnemyMove>().usingNav = true;
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        }
    }

    public void HitFlash()
    {
        StartCoroutine(DoHitFlash());
    }

    public void HitColorFlash()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            fadeToColor = false;
        }
        
        coroutine = StartCoroutine(DoHitColourFlash());
        if (enabled)
            StartCoroutine(DoHitColourFlash());
    }

    IEnumerator DoHitFlash()
    {
        //Gets all mesh renderers and skin renderers
        for (int i = 0; i <= amountToFlash; i++)
        {
            DisableRenderers();
            yield return new WaitForSeconds(timeBetweenFlash);
            EnableRenderers();
            yield return new WaitForSeconds(timeBetweenFlash);
        }
    }

    IEnumerator DoHitColourFlash()
    {
        for (int i = 0; i < amountToFlash; i++)
        {
            SetHitColor();
            yield return new WaitForSeconds(timeBetweenFlash);
            fadeToColor = true;
            yield return new WaitForSeconds(timeBetweenFlash);
            fadeToColor = false;
        }
    }

    public void SetColor(Color color)
    {
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers != null)
        {
            //loops through each and sets the hit color
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
            }
        }
    }

    public void SetColorSeconds(Color color, float seconds)
    {
        StartCoroutine(SetColorCoroutine(color, seconds));
    }

    IEnumerator SetColorCoroutine(Color color, float seconds)
    {
        SetColor(color);
        yield return new WaitForSeconds(seconds);
        SetOGFade();
    }

    public void UnfadeWhite()
    {
        fadeToWhite = true;
    }

    public void SetOGColorRends()
    {
        if (renderers != null)
        {
            //loops through each and sets the hit color
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = Color.white;
            }
        }
    }

    void SetHitColor()
    {
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers != null)
        {
            //loops through each and sets the hit color
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = hitColor;
            }
        }
    }

    public void SetOGColor()
    {
        if (renderers != null)
        {
            if (originalColors.Count > 0)
            {
                //loops through each and sets the hit color
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = originalColors[i];
                }
            }
        }
    }

    void SetOGWhiteFade(float val)
    {
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat("_FlashAmount", renderer.material.GetFloat("_FlashAmount") - val);
                if (renderer.material.GetFloat("_FlashAmount") <= 0)
                {
                    fadeToWhite = false;
                }
            }
        }
    }

    void SetOGFade()
    {
        int rendersCount = 0;
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers != null)
        {
            if (originalColors.Count > 0)
            {
                //loops through each and sets the hit color
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.color != originalColors[i])
                    {
                        renderers[i].material.color = Color.Lerp(renderers[i].material.color, originalColors[i], fadeToColorTime * Time.deltaTime);
                    }

                    if (renderers[i].material.color == originalColors[i])
                    {
                        rendersCount++;
                    }

                    if (rendersCount == renderers.Length)
                    {
                        fadeToColor = false;
                    }
                }

                rendersCount = 0;
            }
        }
    }

    public void SetWhite(float whiteVal)
    {
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat("_FlashAmount", whiteVal);
            }
        }
    }

    public void UnsetWhite()
    {
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat("_FlashAmount", 0);
            }
        }
    }

    public void DisableRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>();
        //loops through each and disables them
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }

    void EnableRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>();
        //loops through each and disables them
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;

            }
        }
    }

    void Update()
    {
        //makes sure health doesn't go below 0        
        if (health <= 0)
        {
            health = 0;
            isDead = true;
            Death();
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (fadeToColor)
        {
            SetOGFade();
        }

        if (fadeToWhite)
        {
            if (GetComponent<PlayerAttack>())
                SetOGWhiteFade(GetComponent<PlayerAttack>().amountOfFadeBack);
        }
    }

    public void Damaged()
    {
        if (animator)
        {
            //animator.SetTrigger ("Hit");
        }
    }

    public bool HasStatusCondition()
    {
        if (isBurned || isPoisoned || isSlowlyDying || isSandy || isFrozen)
        {
            return true;
        }
        return false;
    }

    public void Death()
    {
        //do death
        //checks if the game has an enemy drop script, if it does it is an enemy
        if (!IsEnemy)
        {
            if (animator)
                animator.SetBool("Die", true);
        }
    }

    void DoParticle(string particleName, float duration)
    {
        GameObject particle = Instantiate(Resources.Load<GameObject>(particleName), transform.position, Quaternion.Euler(0, 0, 0));
        particle.GetComponent<ParticleFollowHost>().host = transform;
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ps.Stop();
        ParticleSystem.MainModule main = ps.main;
        main.duration = duration * main.simulationSpeed;
        ps.Play();
        Destroy(particle, duration + 2);
    }

    bool canBeDamagedFromEffect()
    {
        //make sure its a player
        if (!IsEnemy && !isProp)
        {
            //check if they have any orbs
            if (playerInfo.currentCureAmount > 0)
            {
                playerInfo.currentCureAmount -= playerInfo.cureAmountUsedPerTick;
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Sets the poison.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenPoison">Time between poison in seconds.</param>
    public void SetPoison(float damagePerTick, float duration, float timeBetweenPoison)
    {
        if (playerInfo)
            damagePerTick *= playerInfo.GetCharmFloat("poisonMultiplier");
        
        isPoisoned = true;

        DoParticle("PoisonTickParticle", duration);

        StartCoroutine(doPoison(damagePerTick, duration, timeBetweenPoison));

        SoundManager.PlaySound(poisonedSound, transform.position);
    }

    IEnumerator doPoison(float damagePerTick, float duration, float timeBetweenPoison)
    {
        int counter = 0;
        while (isPoisoned)
        {
            counter++;
            SetColor(poisonColor);

            yield return new WaitForSeconds(timeBetweenPoison / 2);

            if (canBeDamagedFromEffect())
            {
                AffectHealth(-damagePerTick);

                SoundManager.PlaySound(poisonTickSound, transform.position);
            }

            StartCoroutine(DisablePlayerFor(.1f));

            if (animator)
                animator.SetTrigger("Flinch");

            if (counter >= duration)
            {
                isPoisoned = false;
            }
            yield return new WaitForSeconds(timeBetweenPoison);
        }
        SetOGColorRends();

        SoundManager.PlaySound(unpoisonedSound, transform.position);
    }

    /// <summary>
    /// Sets the burn.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenBurn">Time between burn in seconds.</param>
    public void SetBurned(float damagePerTick, float duration, float timeBetweenBurn)
    {
        if (!isBurned)
        {
            if (playerInfo)
            {
                damagePerTick = damagePerTick * playerInfo.GetCharmFloat("burnMultiplier");
                if (ItemsManager.Instance.hasArmourPiece)
                {
                    damagePerTick = 0;
                }
            }

            isBurned = true;

            DoParticle("FireTickParticle", duration);

            if (enabled)
                StartCoroutine(doBurn(damagePerTick, duration, timeBetweenBurn));

            SoundManager.PlaySound(burnSound, transform.position);
        }
    }

    IEnumerator doBurn(float damagePerTick, float duration, float timeBetweenBurn)
    {
        int counter = 0;
        while (isBurned)
        {
            counter++;
            SetColor(burnColor);

            yield return new WaitForSeconds(timeBetweenBurn / 2);

            if (canBeDamagedFromEffect())
            {
                AffectHealth(-damagePerTick);

                SoundManager.PlaySound(burnTickSound, transform.position);
            }

            if (gameObject.activeSelf)
                StartCoroutine(DisablePlayerFor(.2f));

            if (animator)
                animator.SetTrigger("Flinch");

            if (counter >= duration)
            {
                isBurned = false;
            }

            yield return new WaitForSeconds(timeBetweenBurn / 2);
        }

        SetOGFade();

        SoundManager.PlaySound(unburnSound, transform.position);
    }

    /// <summary>
    /// Sets the slow death.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenDeathTick">Time between death tick in seconds.</param>
    public void SetSlowDeath(float damagePerTick = .2f, float duration = 1, float timeBetweenDeathTick = 1)
    {
        if (playerInfo)
            damagePerTick *= playerInfo.GetCharmFloat("deathTickMultiplier");
        
        isSlowlyDying = true;

        StartCoroutine(doSlowDeath(damagePerTick, duration, timeBetweenDeathTick));

        SoundManager.PlaySound(slowDeathSound, transform.position);
    }

    IEnumerator doSlowDeath(float damagePerTick, float duration, float timeBetweenSlowDeath)
    {
        int counter = 0;
        while (isSlowlyDying)
        {
            counter++;
            SetColor(slowlyDyingColor);

            yield return new WaitForSeconds(timeBetweenSlowDeath / 2);

            if (canBeDamagedFromEffect())
            {
                AffectHealth(-damagePerTick);

                SoundManager.PlaySound(slowDeathTickSound, transform.position);
            }

            StartCoroutine(DisablePlayerFor(.2f));

            if (animator)
                animator.SetTrigger("Flinch");

            if (counter >= duration)
            {
                isSlowlyDying = false;
            }
            yield return new WaitForSeconds(timeBetweenSlowDeath / 2);
        }

        SetOGFade();

        SoundManager.PlaySound(unslowDeathSound, transform.position);
    }

    /// <summary>
    /// Sets to ice.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    public void SetIce(float duration)
    {
        isFrozen = true;

        DoParticle("IceTickParticle", duration);

        StartCoroutine(doIce(duration));
    }

    IEnumerator doIce(float duration)
    {
        if (canBeDamagedFromEffect())
        {
            //sets the color
            SetColor(frozenColor);
            //disables animator
            animator.enabled = false;

            //do sound
            SoundManager.PlaySound(frozenSound, transform.position);

            //disable move script
            if (GetComponent<PlayerMove>())
                GetComponent<PlayerMove>().enabled = false;
            else if (GetComponent<EnemyMove>())
                GetComponent<EnemyMove>().enabled = false;

            yield return new WaitForSeconds(duration);

            //enable move script
            animator.enabled = true;
            if (GetComponent<PlayerMove>())
                GetComponent<PlayerMove>().enabled = true;
            else if (GetComponent<EnemyMove>())
                GetComponent<EnemyMove>().enabled = true;

            isFrozen = false;

            SetOGFade();

            SoundManager.PlaySound(unfrozenSound, transform.position);
        }
    }

    /// Sets slow speed.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    public void SetSandy(float duration, float speedDamping)
    {
        isSandy = true;

        DoParticle("SandTickParticle", duration);

        StartCoroutine(doSandy(duration, speedDamping));
    }

    IEnumerator doSandy(float duration, float speedDamping)
    {
        if (canBeDamagedFromEffect())
        {
            int counter = 0;
            float ogEnemySpeed = 0;

            //checks if player or enemy and sets the speeds
            if (!IsEnemy)
                playerInfo.SetMoveSpeed(playerInfo.maxMoveSpeed * speedDamping);
            else
            {
                ogEnemySpeed = GetComponent<NavMeshAgent>().speed;
                GetComponent<NavMeshAgent>().speed = GetComponent<NavMeshAgent>().speed * speedDamping;
            }

            SoundManager.PlaySound(sandySound, transform.position);

            while (isSandy)
            {
                counter++;
                SetColor(sandyColor);
                if (counter >= duration)
                {
                    isSandy = false;
                }
                yield return new WaitForSeconds(1);
            }

            SetOGFade();

            SoundManager.PlaySound(unsandySound, transform.position);

            if (!IsEnemy)
                playerInfo.ResetMoveSpeed();
            else
                GetComponent<NavMeshAgent>().speed = ogEnemySpeed;
        }
    }

    IEnumerator DisablePlayerFor(float seconds)
    {
        if (playerInfo)
        {
            PlayerMove move = GetComponent<PlayerMove>();
            PlayerAttack attack = GetComponent<PlayerAttack>();
            move.enabled = false;
            attack.enabled = false;
            yield return new WaitForSeconds(seconds);
            move.enabled = true;
            attack.enabled = true;
        }
    }
}
