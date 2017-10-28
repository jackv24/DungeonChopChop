using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcefieldCol : MonoBehaviour {

    public bool TownForceField = false;
    private Health playerHealth;

	// Use this for initialization
	void Start () 
    {
        playerHealth = transform.parent.GetComponent<Health>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 11 || col.tag == "Projectile")
        {
            playerHealth.TemporaryInvincibility();
            gameObject.SetActive(false);
        }
        if (TownForceField)
        {
            //get the projectile layer
            if (col.GetComponent<Collider>().gameObject.layer == 10)
            {
                Destroy(gameObject);
            }
        }
    }
}
