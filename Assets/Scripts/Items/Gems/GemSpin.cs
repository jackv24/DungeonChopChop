using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpin : MonoBehaviour {

    public float rotationSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.eulerAngles += new Vector3(0, rotationSpeed, 0);
	}
}
