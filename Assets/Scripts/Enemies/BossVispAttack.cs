using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVispAttack : EnemyAttack {

    [Header("Boss Visp Values")]

    [Header("Strike Values")]
    public float minTimeBetweenStrike = 2;
    public float maxTimeBetweenStrike = 4;

    [Header("Spawn Visp Values")]
    public GameObject visp;
    public float minTimeBetweenSpawn = 5;
    public float maxTimeBetweenSpawn = 8;
    public int amountPerSpawn = 1;
    public float rotationSpeed = 2;
    public int maxSpawnedVisps = 10;

    private float timeBetweenShot = 0;
    private float timeBetweenStrike = 0;
    private float timeBetweenSpawn = 0;

    private int shootCounter = 0;
    private int strikeCounter = 0;
    private int spawnCounter = 0;

    private bool striking = false;

    private Animator childAnimator;

    [HideInInspector]
    public VispAttack[] spawnedVisps = new VispAttack[0];

	// Use this for initialization
	void Awake () 
    {
        childAnimator = transform.GetChild(0).GetComponent<Animator>();

        timeBetweenStrike = Random.Range(minTimeBetweenStrike, maxTimeBetweenStrike);
        timeBetweenSpawn = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);
	}

    void FixedUpdate()
    {
        enemyMove.FollowPlayer();

        spawnCounter++;

        if (!striking)
            strikeCounter++;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!striking)
        {
            Vector3 direction = enemyMove.GetClosestPlayer().position - transform.parent.position;

            direction = new Vector3(direction.x, 0, direction.z);

            Vector3 rot = Vector3.RotateTowards(transform.parent.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);

            //look at the player
            transform.parent.rotation = Quaternion.LookRotation(rot);

            //transform.parent.LookAt(enemyMove.GetClosestPlayer().transform, Vector3.up);

            enemyMove.RunAwayFromPlayer(true, 10);

            if (strikeCounter > timeBetweenStrike * 60)
            {
                strikeCounter = 0;

                StartCoroutine(BoolFlick("Strike"));

                timeBetweenStrike = Random.Range(minTimeBetweenStrike, maxTimeBetweenStrike);
            }
        }

        spawnCounter++;

        if (spawnCounter > timeBetweenSpawn * 60)
        {
            spawnCounter = 0;

            if (spawnedVisps.Length < maxSpawnedVisps)
            {
                SplitEnemy(visp, amountPerSpawn, false);
                spawnedVisps = FindObjectsOfType<VispAttack>();
            }

            timeBetweenSpawn = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Striking"))
            striking = true;
        else
            striking = false;
	}

    IEnumerator BoolFlick(string boolname)
    {
        animator.SetBool(boolname, true);
        yield return new WaitForEndOfFrame();
        animator.SetBool(boolname, false);
    }
}
