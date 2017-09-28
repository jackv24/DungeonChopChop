using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BispMove : EnemyMove {

    void Start()
    {
        Setup();
    }
	
	// Update is called once per frame
	void Update () {
        FollowPlayer();
	}
}
