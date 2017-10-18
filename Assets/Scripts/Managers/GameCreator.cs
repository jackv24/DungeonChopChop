using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCreator : MonoBehaviour {

    public bool buildMode = false;
    public GameObject[] managers;

	// Use this for initialization
	void Awake () {
        //create all the managers
        if (!buildMode)
        {
            foreach (GameObject manager in managers)
            {
                Instantiate(manager, transform.position, Quaternion.Euler(0, 0, 0));
            }
        }
        else
        {
            Instantiate(managers[managers.Length - 1], transform.position, Quaternion.Euler(0, 0, 0));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
