using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCreator : MonoBehaviour {

    public GameObject[] managers;

	// Use this for initialization
	void Awake () {
        //create all the managers
        foreach (GameObject manager in managers)
        {
            Instantiate(manager, transform.position, Quaternion.Euler(0, 0, 0));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
