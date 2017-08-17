using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcefieldCol : MonoBehaviour {

    private Health playerHealth;

	// Use this for initialization
	void Start () 
    {
        playerHealth = transform.parent.GetComponent<Health>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy" || col.tag == "Projectile")
        {
            playerHealth.TemporaryInvincibility();
            gameObject.SetActive(false);
        }
    }
}
