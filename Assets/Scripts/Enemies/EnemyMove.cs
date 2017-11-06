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
    [Tooltip("Time between roam change")]
    public float timeBetweenRoam = 3;
    public float runAwayAfterAttackTime = 1;
    public LayerMask enemyMask;
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

    private Vector3 destination;

    void Start()
    {
        Setup();

        if (agent)
            originalSpeed = agent.speed;
    }

    public virtual void OnEnable()
    {
        //sets the time between so we don't have stallers when they spawn
        timeBetweenRoam = 0;

        players = FindObjectsOfType<PlayerInformation>();

        usingNav = true;

        if (enemyHealth)
            enemyHealth.health = enemyHealth.maxHealth;

        //resets the speed so we don't have quick enemies
        if (agent)
        {
            agent.speed = originalSpeed;
            agent.velocity -= agent.velocity;

            if (agent.isOnNavMesh)
                agent.ResetPath();
        }

        if (animator)
            animator.enabled = true;

        ChangeMoveSpeed();
    }

    void ChangeMoveSpeed()
    {
        if (agent)
            agent.speed = originalSpeed * GameManager.Instance.enemyMoveMultiplier;
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

        //gets the agents current destination
        if (agent)
        {
            if (agent.destination != null && destination != agent.destination)
            {
                destination = agent.destination;
            }
        }

        //if the agent reaches the destination, get another desti
        if (destination == agent.transform.position)
            GoToRandomPosition();
    }

    public virtual void FixedUpdate()
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

    protected void GoToTarget(Vector3 target)
    {
        if (canMove)
        {
            if (usingNav)
            {
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(target);
                }
            }
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
                    if (agent)
                    {
                        if (agent.isOnNavMesh)
                        {
                            if (!GetClosestPlayer().GetComponent<Health>().isDead)
                                agent.SetDestination(GetClosestPlayer().position);
                            else
                            {
                                if (OtherwiseRoam)
                                    Roam();
                            }
                        }
                    }
                }
                else
                {
                    if (OtherwiseRoam)
                        Roam();
                }
            }
        }
        else
        {
            if (agent.isOnNavMesh)
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

    protected void Roam()
    { 
        if (canMove)
        {
            roamCounter++; 
            //a simple counter to stop recurring every frame
            if (roamCounter > timeBetweenRoam * 60)
            { 
                timeBetweenRoam = Random.Range(3 / 2, 3 * 1.5f);
                //roams to a random position on the current tile

                GoToRandomPosition();

                roamCounter = 0; 
            } 
        }
        else
        {
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }
    }

    void GoToRandomPosition()
    {
        if (usingNav)
        {
            if (agent.isOnNavMesh)
            {
                if (LevelGenerator.Instance)
                    agent.SetDestination(LevelGenerator.Instance.currentTile.GetPosInTile(1, 1)); 
            }
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

    protected void RunAwayFromPlayer(bool lookAtPlayer)
    {
        if (agent)
        {

            Vector3 newPosition = -transform.forward * 3;

            //rotates away from the player
            if (lookAtPlayer)
                transform.LookAt(GetClosestPlayer());
                
            if (usingNav)
            {
                if (agent.isOnNavMesh)
                    agent.SetDestination(newPosition);
            }
        }
    }

    protected Transform GetClosestPlayerRadius(float radius)
    {
        float distance = Vector3.Distance(transform.position, GetClosestPlayer().transform.position);

        if (distance <= radius)
            return GetClosestPlayer().transform;

        return null;
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
                if (!player.playerMove.playerHealth.isDead)
                    currentPlayer = player;
            }
            previousPlayerDistance = distance;
        }
        if (currentPlayer)
            return currentPlayer.transform;
        else
            return transform;
    }

    public Transform GetClosestEnemyRadius(float radius)
    {
		Collider[] enemies = Physics.OverlapSphere(transform.position, 500, enemyMask);
        foreach (Collider enemy in enemies)
        {
            if (enemy != GetComponent<Collider>())
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < radius)
                {
                    return enemy.transform;
                }
            }
        }
        return transform;
    }

    public Transform GetClosestEnemy()
    {
        GameObject closestEnemy = null;
        float maxDistance = float.MaxValue;
        //gets all enemies in a specific radius
		Collider[] enemies = Physics.OverlapSphere(transform.position, 500, enemyMask);
        foreach (Collider enemy in enemies)
        {
            //loops through each enemy and finds which enemy is closest
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < maxDistance)
            {
                //check if the closest enemy is not this enemy or the player
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
        else if (enemies.Length > 2)
            return enemies[2].transform;
        else
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
