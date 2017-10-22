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
            if (!ManagerExists(manager.name))
            {
                GameObject obj = Instantiate(manager, transform.position, Quaternion.Euler(0, 0, 0));
                obj.name = manager.name;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    bool ManagerExists(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj)
        {
            return true;
        }
        return false;
    }
}
