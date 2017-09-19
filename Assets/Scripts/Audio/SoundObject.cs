using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour {

    AudioSource AS;

	// Use this for initialization
	void Start () {
        AS = GetComponent<AudioSource>();
	}

    void OnEnable()
    {
        AS = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (AS)
        {
            if (!AS.isPlaying)
            {
                Destroy(gameObject);
            }
        }
	}
}
