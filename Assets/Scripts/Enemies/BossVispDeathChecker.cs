using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVispDeathChecker : MonoBehaviour {

    private GameObject child;

	// Use this for initialization
	void Start () 
    {
        child = transform.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!child.gameObject.activeSelf)
            gameObject.SetActive(false);
	}
}
