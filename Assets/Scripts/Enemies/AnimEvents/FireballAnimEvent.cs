using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAnimEvent : MonoBehaviour {

    private FireballMove fireballMove;

	// Use this for initialization
	void Start () 
    {
        fireballMove = GetComponentInParent<FireballMove>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FireTrail()
    {
        fireballMove.DropFireTrail();
    }
}
