using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Space()]
    public bool isPoisoned = false;
    public bool isBurned = false;
    public bool isSlowlyDying = false;

    [Space()]
    public bool isDead = false;

    [Header("Tick Colors")]
    public Color poisonColor;
    public Color burnColor;
    public Color slowlyDyingColor;

    [Space()]
    public AmountOfParticleTypes[] hitParticles;
    public Color hitColor;

    [Space()]
    [Header("Other Vals")]
    public float timeBetweenFlash = 0.1f;
    public int amountToFlash = 5;

    private PlayerInformation playerInfo;
    private Animator animator;
    private Rigidbody rb;

    private Renderer[] renderers;
    private List<Color> originalColors = new List<Color>();
    private Vector3 targetPosition;
    private SpawnEffects spawnEffects;

    void Start()
    {
        spawnEffects = GameObject.FindObjectOfType<SpawnEffects>();

        renderers = GetComponentsInChildren<Renderer>();
        //loops through and get the original color on each renderer
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors.Add(renderers[i].material.color);
        }

        rb = GetComponent<Rigidbody>();

        OnHealthChange += TemporaryInvincibility;

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
            //if (animator)
            //{
            //    animator.SetTrigger("GetHit");
            //}
            if (!HasStatusCondition())
            {
                DoHitParticle();
                HitColorFlash();
            }
        }
    }

    void DoHitParticle()
    {
        if (hitParticles.Length > 0)
            spawnEffects.EffectOnHit(hitParticles, transform.position);
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
        for (int i = 0; i <= amountToFlash; i++)
        {
            SetHitColor();
            yield return new WaitForSeconds(timeBetweenFlash);
            SetOGColor();
            yield return new WaitForSeconds(timeBetweenFlash);
        }
    }

    void SetColor(Color color)
    {
        if (renderers != null)
        {
            //loops through each and sets the hit color
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
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

    void SetOGColor()
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

    void DisableRenderers()
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
        if (isBurned || isPoisoned || isSlowlyDying)
        {
            return true;
        }
        return false;
    }

    public void Death()
    {
        //do death
        //checks if the game has an enemy drop script, if it does it is an enemy
        if (IsEnemy)
        {
            if (transform.GetComponent<Drops>())
            {
                transform.GetComponent<Drops>().DoDrop();
            }
        }
        else
        {
            animator.SetTrigger("Die");
            StartCoroutine(WaitForRestart());
        }
    }

    IEnumerator WaitForRestart()
    {
        yield return new WaitForSeconds(2);
        ObjectPooler.PurgePools();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
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

    /// <summary>
    /// Sets the poison.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenPoison">Time between poison in seconds.</param>
    public void SetPoison(float damagePerTick, float duration, float timeBetweenPoison)
    {
        if (playerInfo)
            damagePerTick = playerInfo.GetCharmFloat("poisonMultiplier");
        isPoisoned = true;
        DoParticle("PoisonTickParticle", duration);
        StartCoroutine(doPoison(damagePerTick, duration, timeBetweenPoison));
    }

    IEnumerator doPoison(float damagePerTick, float duration, float timeBetweenPoison)
    {
        int counter = 0;
        while (isPoisoned)
        {
            counter++;
            SetColor(poisonColor);
            yield return new WaitForSeconds(timeBetweenPoison / 2);
            SetOGColor();
            AffectHealth(-damagePerTick);
            animator.SetTrigger("Flinch");
            if (counter >= duration)
            {
                isPoisoned = false;
            }
            yield return new WaitForSeconds(timeBetweenPoison);
        }
    }

    /// <summary>
    /// Sets the burn.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenBurn">Time between burn in seconds.</param>
    public void SetBurned(float damagePerTick, float duration, float timeBetweenBurn)
    {
        if (playerInfo)
            damagePerTick = damagePerTick * playerInfo.GetCharmFloat("burnMultiplier");
        isBurned = true;
        DoParticle("FireTickParticle", duration);
        StartCoroutine(doBurn(damagePerTick, duration, timeBetweenBurn));
    }

    IEnumerator doBurn(float damagePerTick, float duration, float timeBetweenBurn)
    {
        int counter = 0;
        while (isBurned)
        {
            counter++;
            SetColor(burnColor);
            yield return new WaitForSeconds(timeBetweenBurn / 2);

            AffectHealth(-damagePerTick);
            animator.SetTrigger("Flinch");
            if (counter >= duration)
            {
                isBurned = false;
            }
            SetOGColor();
            yield return new WaitForSeconds(timeBetweenBurn / 2);
        }
    }

    /// <summary>
    /// Sets the slow death.
    /// </summary>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="timeBetweenDeathTick">Time between death tick in seconds.</param>
    public void SetSlowDeath(float damagePerTick, float duration, float timeBetweenDeathTick)
    {
        if (playerInfo)
            damagePerTick = playerInfo.GetCharmFloat("deathTickMultiplier");
        isSlowlyDying = true;
        StartCoroutine(doSlowDeath(damagePerTick, duration, timeBetweenDeathTick));
    }

    IEnumerator doSlowDeath(float damagePerTick, float duration, float timeBetweenBurn)
    {
        int counter = 0;
        while (isSlowlyDying)
        {
            counter++;
            SetColor(slowlyDyingColor);
            yield return new WaitForSeconds(timeBetweenBurn / 2);
            SetOGColor();
            AffectHealth(-damagePerTick);
            animator.SetTrigger("Flinch");
            if (counter >= duration)
            {
                isSlowlyDying = false;
            }
            yield return new WaitForSeconds(timeBetweenBurn / 2);
        }
    }
}
