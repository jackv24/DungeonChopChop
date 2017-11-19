using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBossAttack : EnemyAttack {

    enum BossAttackType
    {
        Spread,
        Sweep,
        Beam
    };

    public delegate void BossEvent();

    public event BossEvent OnSpreadAttack;
    public event BossEvent OnSweepAttack;
    public event BossEvent OnBeamAttack;
    public event BossEvent OnKnockout;
    public event BossEvent OnKnockoutRecover;

    [Header("Big Boss Values")]
    public float timeBetweenAttacks = 2;

    [Header("Sweep Attack Values")]
    public int stage1AmountPerShot = 1;
    public int stage2AmountPerShot = 2;
    public int stage3AmountPerShot = 3;

    [Header("Spread Attack Values")]
    public int amountOfProjectiles = 10;
    public int stage2BurstAmount = 3;
    public int stage3BurstAmount = 6;

    [Header("Knocked Out Values")]
    public int amountTillKnockout;

    [Header("Beam Values")]
    public GameObject beam;
    public float stage1BeamSpeedMulti = 1;
    public float stage2BeamSpeedMulti = 2;
    public float stage3BeamSpeedMulti = 3;

    public int stage = 0;

    private int hitCounter = 0;
    private bool knockedOut = false;
    private ParticleSystem beamParticleSystem;

    private int counter = 0;

    //[Header("Beam Attack Values")]

    void Awake()
    {
        beamParticleSystem = beam.GetComponentInChildren<ParticleSystem>();

        burstFire = false;

        usesChildRotation = true;

        StartCoroutine(wait());
    }

    // Update is called once per frame
    void FixedUpdate () 
    {
            //only count up when the boss is in the idle state
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
        {
            counter++;

            if (counter > timeBetweenAttacks * 60)
            {
                int random = Random.Range(0, 3);

                if (random == 0)
                    Sweep();
                else if (random == 1)
                    Spread();
                else if (random == 2)
                    Beam();

                counter = 0;
            }
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(.2f);

        enemyHealth.OnHealthNegative += CheckHitCount;
    }

    float Percentage(float number, int percent)
    {
        return ((float)number * percent) / 100;
    }

    public void BeamDisable()
    {
        beamParticleSystem.Stop();
    }

    void Beam()
    {
        //beamParticleSystem.Play();

        if (OnBeamAttack != null)
            OnBeamAttack();

        StartCoroutine(boolWait("Beam"));

        StartCoroutine(SetBeamDuration());
    }

    IEnumerator SetBeamDuration()
    {
        beamParticleSystem.Stop();

        while (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Beam"))
            yield return new WaitForEndOfFrame();

        if (stage == 0)
            animator.speed = stage1BeamSpeedMulti;
        else if (stage == 1)
            animator.speed = stage2BeamSpeedMulti;
        else if (stage == 2)
            animator.speed = stage3BeamSpeedMulti;

        ParticleSystem.MainModule main = beamParticleSystem.main;

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Beam"))
        {
            main.duration = 3f * animator.GetCurrentAnimatorStateInfo(0).speedMultiplier;
        }

        beamParticleSystem.Play();
    }

    void Sweep()
    {
        if (OnSweepAttack != null)
            OnSweepAttack();
        
        StartCoroutine(boolWait("Sweep"));
    }

    void Spread()
    {
        if (OnSpreadAttack != null)
            OnSpreadAttack();
        
        StartCoroutine(boolWait("Spread"));
    }

    void Knockout()
    {
        if (OnKnockout != null)
            OnKnockout();
        
        animator.SetBool("KnockedOut", true);
    }

    void CheckHitCount()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("KnockedOut"))
        {
            hitCounter++;

            if (hitCounter >= amountTillKnockout)
            {
                hitCounter = 0;

                Knockout();
                knockedOut = true;
            }
        }

        if (enemyHealth.health <= Percentage(enemyHealth.maxHealth, 25))
            stage = 2;
        else if (enemyHealth.health <= Percentage(enemyHealth.maxHealth, 50))
            stage = 1;
        
    }

    public void RecoveredFromKnockout()
    {
        if (OnKnockoutRecover != null)
            OnKnockoutRecover();
        
        animator.SetBool("KnockedOut", false);
        knockedOut = false;
    }

    IEnumerator boolWait(string b)
    {
        animator.SetBool(b, true);
        yield return new WaitForEndOfFrame();
        animator.SetBool(b, false);
    }

    IEnumerator BurstFire(bool disablesAnimator, BossAttackType attackType, int amount)
    {
        if (disablesAnimator)
            animator.enabled = false;

        for (int i = 0; i < amount; i++)
        {
            if (attackType == BossAttackType.Spread)
                ShootBetweenTwoAngles(-45, 45, amountOfProjectiles, false);
            if (attackType == BossAttackType.Sweep)
                Shootforward();
            
            yield return new WaitForSeconds(timeBetweenShots);
        }

        if (disablesAnimator)
            animator.enabled = true;
    }

    public void SpreadAttack()
    {
        if (stage == 0)
            ShootBetweenTwoAngles(-45, 45, amountOfProjectiles, false);
        else if (stage == 1)
            StartCoroutine(BurstFire(true, BossAttackType.Spread, stage2BurstAmount));
        else if (stage == 2)
            StartCoroutine(BurstFire(true, BossAttackType.Spread, stage3BurstAmount));
    }

    public void SweepAttack()
    {
        if (stage == 0)
            StartCoroutine(BurstFire(false, BossAttackType.Sweep, stage1AmountPerShot));
        else if (stage == 1)
            StartCoroutine(BurstFire(false, BossAttackType.Sweep,stage2AmountPerShot));
        else if (stage == 2)
            StartCoroutine(BurstFire(false, BossAttackType.Sweep,stage3AmountPerShot));
    }
}
