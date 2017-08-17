using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMove : MonoBehaviour 
{

    public TypesOfMoving movingType;

    private NavMeshAgent agent;
    private PlayerInformation[] players;

    //roam vars
    [HideInInspector]
    public float timeBetweenRoam;
    private float roamCounter;

	// Use this for initialization
	void Start () 
    {
        players = FindObjectsOfType<PlayerInformation>();
        agent = GetComponent<NavMeshAgent>();

        roamCounter = timeBetweenRoam * 60;
	}
	
	void FixedUpdate () {
        switch (movingType)
        {
            case TypesOfMoving.Follow:
                FollowPlayer();
                break;
            case TypesOfMoving.Roam:
                Roam();
                break;
        }
	}

    void Roam()
    {
        roamCounter++;
        if (roamCounter > timeBetweenRoam * 60)
        {
            agent.destination = LevelGenerator.Instance.currentTile.GetPosInTile(1, 1);
            roamCounter = 0;
        }
    }

    void FollowPlayer()
    {
        agent.destination = GetClosestPlayer().position;
    }

    Transform GetClosestPlayer()
    {
        float previousPlayerDistance = float.MaxValue;
        PlayerInformation closestPlayer = null;
        foreach (PlayerInformation player in players) 
        {
            float distance = Vector3.Distance (player.transform.position, transform.position);
            if (distance < previousPlayerDistance) 
            {
                closestPlayer = player;
            }
            previousPlayerDistance = distance;
        }
        return closestPlayer.transform;
    }
}
