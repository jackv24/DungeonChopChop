using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SlimeType
{
    Green,
    Blue,
    Red
}

public class SlimeMove : EnemyMove {

    public SlimeType type;

    [Header("Split Values")]
    public bool doesSplit;
    public float splitInterval = 2;
    public float splitAmount;
    [Tooltip("If there are this amount of enemies around, it will not split")]
    public int maxAmountOfSlimes;
    public GameObject slime;
    public float immuneTime = 0;

    [Space()]
    public float damageToEnemies = 1; 

    [Header("Movement Vars")]
    public float followProximity = 20;
    public float radiusAttack = 5;
    public float waitTillLeep = 1;
    [Tooltip("Normal speed multiplier")]
    public float leepSpeed = 2;
    [Space()]
    public float timeBetweenStopMin = 2;
    public float timeBetweenStopMax = 4;
    public float minStopTime = 1;
    public float maxStopTime = 3;

    [Space()]
    public bool friendly = false;

    protected bool inLeeping = false;
    protected bool doingLeep = false;

    protected float splitCounter = 0;

    protected int counter = 0;
    protected float timeBetweenMove = 0;

    void Awake()
    {
        Setup();
    }

    void OnEnable()
    {
        timeBetweenMove = 0;

        agent.enabled = true;

        canMove = true;

        StartCoroutine(immune());

        base.OnEnable();
    }
        
    public override void FixedUpdate()
    {
        if (doesSplit)
        {
            splitCounter++;
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
            {
                if (splitCounter >= (splitInterval * 60))
                {
                    DoSplit();
                    splitCounter = 0;
                }
            }
        }

        counter++;
        if (counter > (timeBetweenMove * 60))
        {
            StartCoroutine(WaitToMove());
            timeBetweenMove = Random.Range(timeBetweenStopMin, timeBetweenStopMax);
            counter = 0;
        }
    }

    IEnumerator immune()
    {
        //immune time, so enemies can't be killed instantly such as boss slimes
        enemyHealth.enabled = false;
        yield return new WaitForSeconds(immuneTime);
        enemyHealth.enabled = true;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!friendly)
        {
            if (!runAway)
            {
                DoMove();
            }
            else
            {
                RunAwayFromPlayer(false);
            }
        }
        else
        {
            FollowEnemy();
        }

        foreach (PlayerInformation player in players)
        {
            if (player.HasCharmBool("slimesAreFriends"))
            {
                friendly = true;
                break;
            }
            else
            {
                friendly = false;
            }
        }
	}

    void DoSplit()
    {
        int currentFrame = ((int)(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (35))) % 35;
        if (currentFrame >= 25)
        {
            for (int i = 1; i <= splitAmount; i++)
            {
                if (CanSplitSlimes())
                {
                    GameObject newSlime = ObjectPooler.GetPooledObject(slime);
                    newSlime.transform.localPosition = transform.localPosition;
                }
            }
        }
    }

    IEnumerator WaitToMove()
    {
        canMove = false;
        float random = Random.Range(minStopTime, maxStopTime);
        yield return new WaitForSeconds(random);
        canMove = true;
    }

    bool CanSplitSlimes()
    {
		BoxCollider[] slimes = (BoxCollider[])Physics.OverlapSphere(transform.position, 500, enemyMask);
        Debug.Log(slimes.Length);
        if (slimes.Length > 0)
        {
            if (slimes.Length < maxAmountOfSlimes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    void DoMove()
    {
        //if red, attack player
        if (type == SlimeType.Red)
        {
            AttackPlayer();
        }
        //if blue, roam until in distance, then attac
        else if (type == SlimeType.Blue)
        {
            if (!InDistance(followProximity))
                Roam();
            else
                AttackPlayer();
        }
        //if green, just roam
        else if (type == SlimeType.Green)
        {
            Roam();
        }
    }

    void AttackPlayer()
    {

        if (!InDistance(radiusAttack))
        {
            FollowPlayer();
        }
        else
        {
            if (!inLeeping)
                StartCoroutine(LeepAtEnemy(waitTillLeep));
        }
        if (doingLeep)
        {
            FollowPlayer();
        }
    }

    IEnumerator LeepAtEnemy(float waitTillLeep)
    {
        //sets some values to be true, which starts the leep
        inLeeping = true;

        //stops the agent
        agent.speed = 0;

        yield return new WaitForSeconds(waitTillLeep);

        //the waiting has finished, now leep
        doingLeep = true;

        //speed the agent up
        agent.speed = originalSpeed * leepSpeed;

        yield return new WaitForSeconds(1);

        //leep is now over, reset the speed
        agent.speed = originalSpeed;

        inLeeping = false;
        doingLeep = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (friendly)
        {
            if (col.GetComponent<Collider>().tag != "Player")
            {
                if (col.gameObject.GetComponent<Health>())
                {
                    col.gameObject.GetComponent<Health>().AffectHealth(-damageToEnemies);
                }
            }
        }
    }
}
