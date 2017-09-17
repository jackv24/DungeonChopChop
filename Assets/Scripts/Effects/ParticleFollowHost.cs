using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowHost : MonoBehaviour {

    public Transform host;
    ParticleSystem ps;
    ParticleSystem.MainModule main;


	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (host)
        {
            transform.position = host.position;
            if (host.GetComponent<Health>().health <= 0)
            {
                ps.Stop();
            }
        }
    }

}
