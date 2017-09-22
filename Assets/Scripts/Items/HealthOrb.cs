using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour {

    public float healthAmount;
    public GameObject particleOnCollect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player1" || col.tag == "Player2")
        {
            if (col.GetComponent<Health>().health < col.GetComponent<Health>().maxHealth)
            {
                col.gameObject.GetComponent<Health>().health += healthAmount;
                GameObject particle = ObjectPooler.GetPooledObject(particleOnCollect);
                particle.GetComponent<ParticleFollowHost>().host = col.transform;
                particle.transform.position = transform.position;
                gameObject.SetActive(false);
            }
        }
    }
}
