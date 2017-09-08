using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowHost : MonoBehaviour {

    public Transform host;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = host.position;
	}
}
