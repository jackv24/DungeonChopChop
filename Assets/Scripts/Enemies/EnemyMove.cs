﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour 
{
    protected NavMeshAgent agent;
    protected PlayerInformation[] players;

    protected PlayerInformation currentPlayer = null;
    protected Animator animator;

    private int roamCounter = 0;

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

    protected void FollowEnemy()
    {
        agent.destination = GetClosestEnemy("Slime").position;
    }

    protected void Roam(float timeBetweenRoam) 
    { 
        roamCounter++; 
        if (roamCounter > timeBetweenRoam * 60) 
        { 
            agent.destination = LevelGenerator.Instance.currentTile.GetPosInTile(1, 1); 
            roamCounter = 0; 
        } 
    } 

    protected bool IsInDistanceOfPlayer(float radius)
    {
        float distance = Vector3.Distance(transform.position, GetClosestPlayer().position);
        if (distance < radius)
        {
            return true;
        }
        return false;
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

    protected Transform GetClosestEnemy(string enemyTag)
    {
        GameObject closestEnemy = null;
        float maxDistance = float.MaxValue;
        Collider[] enemies = Physics.OverlapSphere(transform.position, 100);
        foreach (Collider enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < maxDistance)
            {
                closestEnemy = enemy.gameObject;
                maxDistance = dist;
            }
        }
        return closestEnemy.transform;
    }
}
