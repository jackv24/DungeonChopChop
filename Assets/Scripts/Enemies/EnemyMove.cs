using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour 
{
    private NavMeshAgent agent;
    private PlayerInformation[] players;

    protected PlayerInformation currentPlayer = null;
    protected Animator animator;

    protected void Setup()
    {
        animator = GetComponentInChildren<Animator>();
        players = FindObjectsOfType<PlayerInformation>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected void FollowPlayer()
    {
        agent.destination = GetClosestPlayer().position;
    }

    protected Transform GetClosestPlayer()
    {
        float previousPlayerDistance = float.MaxValue;
        foreach (PlayerInformation player in players) 
        {
            float distance = Vector3.Distance (player.transform.position, transform.position);
            if (distance < previousPlayerDistance) 
            {
                currentPlayer = player;
            }
            previousPlayerDistance = distance;
        }
        return currentPlayer.transform;
    }
}
