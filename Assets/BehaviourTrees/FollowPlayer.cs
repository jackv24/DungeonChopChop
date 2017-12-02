using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.AI;
using NodeCanvas.BehaviourTrees;

public class FollowPlayer : ActionTask<NavMeshAgent> {

    public BBParameter<float> speed = 4;

    private PlayerInformation closestPlayer;

    protected override void OnExecute()
    {
        closestPlayer = blackboard.GetVariable<PlayerInformation>("_closestPlayer").value;
    }

    protected override void OnUpdate()
    {
        agent.SetDestination(closestPlayer.transform.position);
    }
}
