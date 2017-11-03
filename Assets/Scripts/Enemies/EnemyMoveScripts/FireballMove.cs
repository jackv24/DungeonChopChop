using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMove : EnemyMove {

    [Header("Fire ball Values")]
    public GameObject fireTrail;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        FollowPlayer();
	}

    public void DropFireTrail()
    {
        GameObject obj = ObjectPooler.GetPooledObject(fireTrail);
        obj.transform.position = transform.position;
    }
}
