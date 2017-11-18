using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CameraShakeVars
{
    [Header("Camera Shake Values")]
    public float magnitude = .1f;
    public float shakeAmount = .1f;
    public float duration = 1;
}

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float health;
    [Tooltip("The heavier the weight, less knockback")]
    public float weight;

    public delegate void HealthEvent();

    public event HealthEvent OnDeath;
    public event HealthEvent OnHealthChange;

    public event HealthEvent OnHealthNegative;
    public event HealthEvent OnHealthPositive;

    [Space()]
    public bool IsEnemy;
    public EnemyKind enemyKind;
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

    [Space()]
    [Header("Other Vals")]
    public float timeBetweenFlash = 0.1f;
    public int amountToFlash = 5;
    public float fadeTime = 5;

    public CameraShakeVars hitShake;

    [HideInInspector]

    private PlayerInformation playerInfo;
    private Animator animator;
    private Rigidbody rb;

    private List<Renderer> renderers = new List<Renderer>();
    private List<Color> originalColors = new List<Color>();
    private Vector3 targetPosition;

    private Color fadeToColorColor;
    private float fadeToColortime;

    private bool fadeToColor = false;
    private bool unfadeWhite = false;
    private bool fadeToOG = false;

    private GameObject ailmentIcon;

    private Coroutine coroutine;

    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }

    void Start()
    {
        AddRenderersToList();

        //loops through and get the original color on each renderer
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors.Add(renderers[i].material.color);
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

    void Update()
    {
        //makes sure health doesn't go below 0        
        if (health <= 0.0f)
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
            ColorToFade();
        }

        if (unfadeWhite)
        {
            OGWhiteFade(fadeTime);
        }

        if (fadeToOG)
        {
            OGFade(fadeTime);
        }

        //check if the object has an ailment icon but is not ailmented
        if (!HasStatusCondition())
        {
            if (ailmentIcon)
            {
                if (ailmentIcon.activeSelf)
                    ailmentIcon.SetActive(false);
            }
        }
    }

    public void AffectHealth(float healthDeta)
    {
        //make sure this script is enabled
        if (enabled)
        {
            //props don't need to do this
            if (!isProp)
            {
                if (health > 0)
                {
                    TemporaryInvincibility();
                }
            }

            if (healthDeta > 0.0f)
            {
                if (OnHealthPositive != null)
                    OnHealthPositive();
            }
            else
            {
                if (OnHealthNegative != null)
                    OnHealthNegative();
            }

            //add to damage statistics
            if (!IsEnemy && !isProp)
            {
                float val = System.Math.Abs(healthDeta);
                Statistics.Instance.totalDamageTaken += (float)System.Math.Round(val, 2);
            }

            if (col && !isProp)
            {
                float damage = (float)System.Math.Round((double)healthDeta, 1);
                DamageText.Show(damage, col.bounds.center + Vector3.up);
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

            DoHitSoundAndShake();
        }
    }

    void DoHitSoundAndShake()
    {
        if (IsEnemy || isProp)
        {
            if (!HasStatusCondition())
            {
                DoHitParticle();
                DoHitSound();
                HitColorFlash();
                CameraShake.ShakeScreen(hitShake.magnitude, hitShake.shakeAmount, hitShake.duration);
            }
        }
        else if (!IsEnemy)
        {
            if (!HasStatusCondition())
            {
                if (animator)
                    animator.SetTrigger("Hit");
                DoHitParticle();
                CameraShake.ShakeScreen(hitShake.magnitude, hitShake.shakeAmount, hitShake.duration);
				ControllerRumble.RumbleController(playerInfo.playerMove.input, hitShake.magnitude, hitShake.duration);
            }

            DoHitSound();
        }
    }

    void AddRenderersToList()
    {
        renderers.Clear();
        foreach (Renderer ren in transform.GetComponentsInChildren<Renderer>())
        {
            //we don't want the trail renderer
            if (ren is TrailRenderer || ren is ParticleSystemRenderer) {}
                //no
            else 
                renderers.Add(ren);
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

        UnFreeze();

		if(GameManager.Instance && IsEnemy)
			maxHealth *= GameManager.Instance.enemyHealthMultiplier;

        health = maxHealth;

        isDead = false;
    }

    void OnDisable()
    {
        if (ailmentIcon)
            ailmentIcon.SetActive(false);
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
        yield return new WaitForSeconds(playerInfo.invincibilityTimeAfterHit + playerInfo.GetCharmFloat("invincibleTimeIncrease"));
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
            SetOGFade();
        }
    }

    void AddColorsToList()
    {
        if (!IsEnemy && !isProp)
        {
            originalColors.Clear();

            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                    originalColors.Add(renderers[i].material.color);
            }
        }
    }

    public void SetColor(Color color)
    {
        AddRenderersToList();

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
        AddRenderersToList();

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
        AddRenderersToList();

        if (renderers != null)
        {
            if (originalColors.Count > 0)
            {
                //loops through each and sets the hit color
                for (int i = 0; i < originalColors.Count; i++)
                {
                    if (renderers[i].material.HasProperty("_Color"))
                    {
                        renderers[i].material.color = originalColors[i];
                    }
                }
            }
        }
    }

    void ResetColour()
    {
        AddRenderersToList();

        foreach (Renderer ren in renderers)
        {
            ren.material.color = Color.white;
        }
    }

    void SetColorToFade(Color col, float fadeTime)
    {
        fadeToColorColor = col;

        fadeToColortime = fadeTime;

        fadeToColor = true;
    }

    void ColorToFade()
    {
        if (renderers != null)
        {
            int renderersCount = 0;

            foreach (Renderer renderer in renderers)
            {
                //loop through and fade the renderers colour
                if (renderer.material.color != fadeToColorColor)
                {
                    renderer.material.color = Color.Lerp(renderer.material.color, fadeToColorColor, fadeToColortime* Time.deltaTime);
                }
                else
                {
                    renderersCount++;
                }

                //check if all the renderers have finished changing colour
                if (renderersCount == renderers.Count)
                {
                    fadeToColor = false;
                }
            }
        }
    }

    public void SetOGWhiteFade()
    {
        unfadeWhite = true;
    }

    void OGWhiteFade(float val)
    {
        if (renderers != null)
        {
            //loops through all the renderers and lerps the player to white
            foreach (Renderer renderer in renderers)
            {
                if (renderer.material.GetFloat("_FlashAmount") > 0)
                {
                    renderer.material.SetFloat("_FlashAmount", renderer.material.GetFloat("_FlashAmount") - fadeTime / 25);
                }
                else
                {
                    //just to make sure the value doesn't go into a negative state
                    renderer.material.SetFloat("_FlashAmount", 0);
                    unfadeWhite = false;
                }
            }
        }
    }

    void SetOGFade()
    {
        fadeToOG = true;
    }

    void OGFade(float fadeTime)
    {
        AddRenderersToList();

        if (renderers != null)
        {
            if (originalColors.Count > 0)
            {
                int rendersCount = 0;

                //loops through each and sets the hit color
                for (int i = 0; i < renderers.Count; i++)
                {
                    if (renderers[i].material.color != originalColors[i])
                    {
                        renderers[i].material.color = Color.Lerp(renderers[i].material.color, originalColors[i], fadeTime * Time.deltaTime);
                    }

                    if (renderers[i].material.color == originalColors[i])
                    {
                        rendersCount++;
                    }

                    if (rendersCount == renderers.Count)
                    {
                        fadeToOG = false;
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
        AddRenderersToList();

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
        AddRenderersToList();

        //loops through each and disables them
        if (renderers != null)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;

            }
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
        if (!IsEnemy && !isProp)
        {
            if (animator)
            {
                animator.SetBool("Die", true);

                playerInfo.playerMove.enabled = false;
                playerInfo.characterController.enabled = false;
            }
        }
    }

    void DoParticle(string particleName, float duration)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject particle = Instantiate(Resources.Load<GameObject>(particleName), transform.position, Quaternion.Euler(0, 0, 0));
            particle.GetComponent<ParticleFollowHost>().host = transform;

            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            ps.Stop();

            ParticleSystem.MainModule main = ps.main;
            main.duration = duration * main.simulationSpeed;

            if (i == 1)
                main.simulationSpace = ParticleSystemSimulationSpace.Local;

            ps.Play();
            Destroy(particle, duration + 2);
        }
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

    void CreateAilmentIcon(string canvasName)
    {
        if (!isProp)
        {
            //creates the icon above the player/enemy to show its ailmented
            ailmentIcon = ObjectPooler.GetPooledObject(Resources.Load<GameObject>(canvasName));

            ailmentIcon.transform.position = new Vector3(transform.position.x, GetComponent<Collider>().bounds.max.y, transform.position.z);

            ailmentIcon.GetComponent<AilmentIcon>().host = gameObject;
        }
    }

    public void RemoveAilment()
    {
        isBurned = false;
        isFrozen = false;
        isSandy = false;
        isSlowlyDying = false;
        isPoisoned = false;
    }

    bool CanBeAilmented(StatusType type)
    {
        if (GetComponent<ImmuneTo>())
        {
            if (GetComponent<ImmuneTo>().type == type)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Sets the poison.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenPoison">Time between poison in seconds.</param>
    public void SetPoison(float damagePerTick = .2f, float duration = 3, float timeBetweenPoison = 1)
    {
        if (CanBeAilmented(StatusType.poison))
        {
            if (playerInfo)
                damagePerTick *= playerInfo.GetCharmFloat("poisonMultiplier");

            CreateAilmentIcon("PoisonCanvas");
        
            isPoisoned = true;

            DoParticle("PoisonTickParticle", duration);

            StartCoroutine(doPoison(damagePerTick, duration, timeBetweenPoison));

            SoundManager.PlayAilmentSound(StatusType.poison, ailmentSoundType.Start, transform.position);
        }
    }

    IEnumerator doPoison(float damagePerTick, float duration, float timeBetweenPoison)
    {
        float finishTime = Time.time + duration;

        while (isPoisoned)
        {
            SetColor(poisonColor);

            yield return new WaitForSeconds(timeBetweenPoison / 2);

            AffectHealth(-damagePerTick);

            SoundManager.PlayAilmentSound(StatusType.poison, ailmentSoundType.Tick, transform.position);

            StartCoroutine(DisablePlayerFor(.1f));

            if (animator)
                animator.SetTrigger("Flinch");

            if (Time.time >= finishTime)
            {
                isPoisoned = false;
            }
            yield return new WaitForSeconds(timeBetweenPoison);
        }

        ailmentIcon.SetActive(false);

        SetOGFade();

        SoundManager.PlayAilmentSound(StatusType.poison, ailmentSoundType.End, transform.position);

        //a safety in case the player gets stuck the wrong color;
        if (!IsEnemy && !isProp)
        {
            yield return new WaitForSeconds(5);
            ResetColour();
        }
    }

    /// <summary>
    /// Sets the burn.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenBurn">Time between burn in seconds.</param>
    public void SetBurned(float damagePerTick = .2f, float duration = 3, float timeBetweenBurn = 1)
    {
        if (CanBeAilmented(StatusType.burn))
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

                CreateAilmentIcon("FireCanvas");

                isBurned = true;

                DoParticle("FireTickParticle", duration);

                if (enabled)
                    StartCoroutine(doBurn(damagePerTick, duration, timeBetweenBurn));

                SoundManager.PlayAilmentSound(StatusType.burn, ailmentSoundType.Start, transform.position);
            }
        }
    }

    IEnumerator doBurn(float damagePerTick, float duration, float timeBetweenBurn)
    {
        float finishTime = Time.time + duration;

        if (!isProp)
            SetColor(burnColor);

        while (isBurned)
        {
            yield return new WaitForSeconds(timeBetweenBurn / 2);

            AffectHealth(-damagePerTick);

            SoundManager.PlayAilmentSound(StatusType.burn, ailmentSoundType.Tick, transform.position);

            if (isProp)
                SetColorToFade(burnColor, fadeTime);

            if (gameObject.activeSelf)
                StartCoroutine(DisablePlayerFor(.2f));

            if (!IsEnemy && !isProp)
                animator.SetTrigger("Flinch");

            if (Time.time >= finishTime)
            {
                isBurned = false;
            }

            yield return new WaitForSeconds(timeBetweenBurn / 2);
           
        }

        if (!isProp)
            SetOGFade();

        ailmentIcon.SetActive(false);

        SoundManager.PlayAilmentSound(StatusType.burn, ailmentSoundType.End, transform.position);

        //a safety in case the player gets stuck the wrong color;
        if (!IsEnemy && !isProp)
        {
            yield return new WaitForSeconds(5);
            ResetColour();
        }
    }

    /// <summary>
    /// Sets the slow death.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenDeathTick">Time between death tick in seconds.</param>
    public void SetSlowDeath(float damagePerTick = .2f, float duration = 1, float timeBetweenDeathTick = 1)
    {
        if (CanBeAilmented(StatusType.slowlyDying))
        {
            if (playerInfo)
                damagePerTick *= playerInfo.GetCharmFloat("deathTickMultiplier");
        
            isSlowlyDying = true;

            CreateAilmentIcon("InfectedCanvas");

            StartCoroutine(doSlowDeath(damagePerTick, duration, timeBetweenDeathTick));

            SoundManager.PlayAilmentSound(StatusType.slowlyDying, ailmentSoundType.Start, transform.position);
        }
    }

    IEnumerator doSlowDeath(float damagePerTick, float duration, float timeBetweenSlowDeath)
    {
        float finishTime = Time.time + duration;

        while (isSlowlyDying)
        {
            SetColor(slowlyDyingColor);

            yield return new WaitForSeconds(timeBetweenSlowDeath / 2);

            AffectHealth(-damagePerTick);

            SoundManager.PlayAilmentSound(StatusType.slowlyDying, ailmentSoundType.Tick, transform.position);

            StartCoroutine(DisablePlayerFor(.2f));

            if (animator)
                animator.SetTrigger("Flinch");

            if (Time.time >= finishTime)
            {
                isSlowlyDying = false;
            }
            yield return new WaitForSeconds(timeBetweenSlowDeath / 2);
        }

        ailmentIcon.SetActive(false);

        SetOGFade();

        SoundManager.PlayAilmentSound(StatusType.slowlyDying, ailmentSoundType.End, transform.position);

        //a safety in case the player gets stuck the wrong color;
        if (!IsEnemy && !isProp)
        {
            yield return new WaitForSeconds(5);
            ResetColour();
        }
    }

    /// <summary>
    /// Sets to ice.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    public void SetIce(float duration = 1)
    {
        if (CanBeAilmented(StatusType.Ice))
        {
            isFrozen = true;

            DoParticle("IceTickParticle", duration);

            CreateAilmentIcon("IceCanvas");

            StartCoroutine(doIce(duration));
        }
    }

    IEnumerator doIce(float duration)
    {
        //sets the color
        SetColor(frozenColor);
        //disables animator
        animator.enabled = false;

        //do sound
        SoundManager.PlayAilmentSound(StatusType.Ice, ailmentSoundType.Start, transform.position);

        //disable move script
        if (GetComponent<PlayerMove>())
            GetComponent<PlayerMove>().enabled = false;
        else if (GetComponent<EnemyMove>())
            GetComponent<EnemyMove>().enabled = false;

        yield return new WaitForSeconds(duration);

        isFrozen = false;

        UnFreeze();

        ailmentIcon.SetActive(false);

        SetOGFade();

        SoundManager.PlayAilmentSound(StatusType.Ice, ailmentSoundType.End, transform.position);

        //a safety in case the player gets stuck the wrong color;
        if (!IsEnemy && !isProp)
        {
            yield return new WaitForSeconds(5);
            ResetColour();
        }
    }

    void UnFreeze()
    {
        //enable move script
        if (animator)
            animator.enabled = true;

        if (GetComponent<PlayerInformation>())
            GetComponent<PlayerMove>().enabled = true;
        else if (GetComponent<EnemyMove>())
            GetComponent<EnemyMove>().enabled = true;

        isFrozen = false;
    }

    /// Sets slow speed.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    public void SetSandy(float duration = 1, float speedDamping = .5f)
    {
        if (CanBeAilmented(StatusType.Sandy))
        {
            isSandy = true;

            DoParticle("SandTickParticle", duration);

            CreateAilmentIcon("SlowCanvas");

            StartCoroutine(doSandy(duration, speedDamping));
        }
    }

    IEnumerator doSandy(float duration, float speedDamping)
    {
        if (canBeDamagedFromEffect())
        {
            float ogEnemySpeed = 0;

            //checks if player or enemy and sets the speeds
            if (!IsEnemy)
                playerInfo.SetMoveSpeed(playerInfo.maxMoveSpeed * speedDamping);
            else
            {
                if (GetComponent<NavMeshAgent>())
                {
                    ogEnemySpeed = GetComponent<NavMeshAgent>().speed;
                    GetComponent<NavMeshAgent>().speed = GetComponent<NavMeshAgent>().speed * speedDamping;
                }
            }

            SetColor(sandyColor);

            SoundManager.PlayAilmentSound(StatusType.Sandy, ailmentSoundType.Start, transform.position);

            yield return new WaitForSeconds(duration);

            isSandy = false;

            SetOGFade();

            //a safety in case the player gets stuck the wrong color;
            if (!IsEnemy && !isProp)
            {
                yield return new WaitForSeconds(2);
                ResetColour();
            }

            ailmentIcon.SetActive(false);

            SoundManager.PlayAilmentSound(StatusType.Sandy, ailmentSoundType.End, transform.position);

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
