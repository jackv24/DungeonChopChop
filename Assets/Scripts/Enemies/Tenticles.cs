using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tenticles : MonoBehaviour {

    public float DamageOnTouch = .2f;
    private Quicksand quicksand;

	// Use this for initialization
	void Start () {
        quicksand = GetComponentInParent<Quicksand>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Health>())
            col.GetComponent<Health>().AffectHealth(-.2f);
        
        if (col.gameObject.layer == 16)
        {
            BoxCollider box = col.GetComponent<BoxCollider>();
            if (box.enabled)
            {
                quicksand.SpikesHit();
            }
        }
    }
}
