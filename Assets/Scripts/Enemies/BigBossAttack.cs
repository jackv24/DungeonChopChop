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

    [Header("Knocked Out Values")]
    public int amountTillKnockout;

    public int stage = 0;

    private int hitCounter = 0;
    private bool knockedOut = false;

    private int counter = 0;

    //[Header("Beam Attack Values")]

    void Awake()
    {
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
                int random = Random.Range(0, 2);

                if (random == 0)
                    Sweep();
                else if (random == 1)
                    Spread();

                counter = 0;
            }
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(.2f);

        enemyHealth.OnHealthNegative += CheckHitCount;
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
            StartCoroutine(BurstFire(true, BossAttackType.Spread, burstAmount));
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
