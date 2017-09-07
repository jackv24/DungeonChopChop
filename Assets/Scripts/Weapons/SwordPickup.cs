using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPickup : MonoBehaviour {

    public bool canPickUp;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<PlayerAttack>())
        {
            if (canPickUp)
            {
                PlayerAttack playerAttack = col.GetComponent<PlayerAttack>();
                playerAttack.AddSword(GetComponent<SwordStats>());
                Debug.Log(transform.name);
            }
        }
    }
}
