using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [Tooltip("All enemies have this radius")]
    public float OverallRadiusFollow = 30;
    [Tooltip("If not in radius, roam or not")]
    public bool OtherwiseRoam = true;
    public float runAwayAfterAttackTime = 1;
    public LayerMask layerMask;
    public bool LockY = true;

    protected float originalSpeed;

    protected NavMeshAgent agent;
    protected PlayerInformation[] players;
    protected PlayerInformation currentPlayer = null;
    protected Animator animator;
    protected EnemyAttack enemyAttack;
    protected Health enemyHealth;

    protected bool runAway = false;
    protected bool canMove = true;
    public bool usingNav = true;
    private int roamCounter = 0;

    void Start()
    {
        Setup();
    }

    void OnEnable()
    {
        usingNav = true;
    }

    void OnDisable()
    {
        if (agent)
            agent.enabled = false;
    }

    void Update()
    {
        if (LockY)
        {
            if (transform.position.y > .14f || transform.position.y < .14f)
            {
                transform.position = new Vector3(transform.position.x, .14f, transform.position.z);
            }
        }
    }

    void FixedUpdate()
    {
        if (currentPlayer)
        {
            //checks if the current player has the following charm
            if (currentPlayer.HasCharmBool("enemiesRunAway"))
            {
                runAway = true;
            }
            else
            {
                runAway = false;
            }
        }
    }

    protected void Setup()
    {
        enemyHealth = GetComponent<Health>();
        enemyAttack = GetComponent<EnemyAttack>();
        animator = GetComponentInChildren<Animator>();
        players = FindObjectsOfType<PlayerInformation>();
        if (GetComponent<NavMeshAgent>())
        {
            agent = GetComponent<NavMeshAgent>();
            originalSpeed = agent.speed;
        }
    }

    protected void FollowPlayer()
    {
        //follows the closest player using nav mesh
        if (canMove)
        {
            if (usingNav)
            {
                if (InDistance(OverallRadiusFollow))
                {
                    if (agent.isOnNavMesh)
                        agent.SetDestination(GetClosestPlayer().position);
                    else
                    {
                        if (OtherwiseRoam)
                            Roam(3);
                    }
                }
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, OverallRadiusFollow);
    }

    protected void FollowEnemy()
    {
        //follows the closest enemy
        if (usingNav)
        {
            if (agent.isOnNavMesh)
                agent.SetDestination(GetClosestEnemy().position);
        }
    }

    protected void Roam(float timeBetweenRoam)
    { 
        if (canMove)
        {
            roamCounter++; 
            //a simple counter to stop recurring every frame
            if (roamCounter > timeBetweenRoam * 60)
            { 
                //roams to a random position on the current tile
                if (usingNav)
                {
                    if (agent.isOnNavMesh)
                        agent.SetDestination(LevelGenerator.Instance.currentTile.GetPosInTile(1, 1)); 
                }
                roamCounter = 0; 
            } 
        }
        else
        {
            agent.ResetPath();
        }
    }

    public bool InDistance(float radius)
    {
        //checks to see if the player is in the radius of the enemy
        float distance = Vector3.Distance(transform.position, GetClosestPlayer().position);
        if (distance < radius)
        {
            return true;
        }
        return false;
    }

    protected float GetDistance()
    {
        return Vector3.Distance(transform.position, GetClosestPlayer().position);
    }

    public bool InDistanceBetweenTwoRadius(float HigherRadius, float LowerRadius)
    {
        //checks to see if the player is in the radius of the enemy
        float distance = Vector3.Distance(transform.position, GetClosestPlayer().position);
        if (distance < HigherRadius && distance > LowerRadius)
        {
            return true;
        }
        return false;
    }

    protected void RunAwayFromPlayer()
    {
        if (GetComponent<NavMeshAgent>())
        {
            //rotates away from the player
            transform.rotation = Quaternion.LookRotation(transform.position - GetClosestPlayer().position);

            //Gets a new vector position in front of the enemy 
            Vector3 runTo = transform.position + transform.forward * 5;

            NavMeshHit hit;

            //checks to make sure the point is reachable on the nav mesh
            NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));

            //moves to that position
            if (usingNav)
            {
                if (agent.isOnNavMesh)
                    agent.SetDestination(hit.position);
            }
        }
    }

    protected Transform GetClosestPlayer()
    {
        float previousPlayerDistance = float.MaxValue;
        foreach (PlayerInformation player in players)
        {
            //loops through both players and finds out which player is closest
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < previousPlayerDistance)
            {
                currentPlayer = player;
            }
            previousPlayerDistance = distance;
        }
        return currentPlayer.transform;
    }

    protected Transform GetClosestEnemy()
    {
        GameObject closestEnemy = null;
        float maxDistance = float.MaxValue;
        //gets all enemies in a specific radius
        Collider[] enemies = Physics.OverlapSphere(transform.position, 500, layerMask);
        foreach (Collider enemy in enemies)
        {
            //loops through each enemy and finds which enemy is closest
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < maxDistance)
            {
                if (enemy != GetComponent<Collider>())
                {
                    if (enemy.gameObject.layer == 11)
                    {
                        closestEnemy = enemy.gameObject;
                    }
                }
                maxDistance = dist;
            }
        }
        //returns the closest enemy
        if (closestEnemy)
            return closestEnemy.transform;
        return transform;
    }

    protected void LookAtClosestPlayer(float rotateSpeed)
    {
        Vector3 rot = Quaternion.LookRotation(GetClosestPlayer().position - transform.position).eulerAngles;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rot), rotateSpeed * Time.deltaTime);
    }

    public void runAwayForSeconds()
    {
        StartCoroutine(runAwayFor());
    }

    IEnumerator runAwayFor()
    {
        runAway = true;
        yield return new WaitForSeconds(runAwayAfterAttackTime);
        runAway = false;
    }
}
