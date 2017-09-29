using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VispMove : EnemyMove {

    [Tooltip("The speed when in the shoot radius (On Visp Attack")]
    public float speedDuringShootRadius = 3.5f;
    [Tooltip("The speed when in the attack radius (On Visp Attack")]
    public float speedDuringAttackRadius = 3.5f;

    private VispAttack vispAttack;

	// Use this for initialization
	void Start () {
        Setup();
        vispAttack = GetComponent<VispAttack>();
	}
	
	// Update is called once per frame
	void Update () {
        
        //sets the speed of the visp depending on the range of the player
        if (InDistanceBetweenTwoRadius(vispAttack.shootRadius, vispAttack.attackRadius))
        {
            agent.speed = speedDuringShootRadius;
        }
        else if (InDistance(vispAttack.attackRadius))
        {
            agent.speed = speedDuringAttackRadius;
        }

        FollowPlayer();
	}
}
