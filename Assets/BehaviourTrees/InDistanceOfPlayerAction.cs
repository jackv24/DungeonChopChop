using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.AI;
using NodeCanvas.BehaviourTrees;

public class InDistanceOfPlayerAction : ConditionTask<NavMeshAgent> {

    public float _distance = 5;
    public bool inDistanceReturnsTrue = true;
    private BBParameter<PlayerInformation> _closestPlayer;

    protected override bool OnCheck()
    {
        return InDistance();
    }

    bool InDistance()
    {
        BBParameter<float> previousPlayerDistance = float.MaxValue;

        PlayerInformation currentPlayer = new PlayerInformation();

        BBParameter<float> distance = 0;

        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            //loops through both players and finds out which player is closest
            distance = Vector3.Distance(player.transform.position, agent.transform.position);

            if (distance.value < previousPlayerDistance.value)
            {
                if (!player.playerMove.playerHealth.isDead)
                    currentPlayer = player;
            }
            previousPlayerDistance.value = distance.value;
        }

        _closestPlayer = currentPlayer;

        Debug.Log(previousPlayerDistance.value);

        if (previousPlayerDistance.value <= _distance)
        {
            if (inDistanceReturnsTrue)
                return true;
            else
                return false;
        }
        else
        {
            if (inDistanceReturnsTrue)
                return false;
            else
                return true;
        }
    }
}
