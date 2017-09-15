using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quicksand : MonoBehaviour {

    public float tickDamage;
    public float timeBetweenDamage;

    private float counter;

	// Use this for initialization
	void Start () {
		
	}
	
    void FixedUpdate()
    {
        counter += .016f;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player1" || col.gameObject.tag == "Player2")
        {
            if (counter >= timeBetweenDamage)
            {
                col.GetComponent<Health>().AffectHealth(-tickDamage);
                counter = 0;
            }
        }
    }
}
