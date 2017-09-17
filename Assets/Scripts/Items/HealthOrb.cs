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

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player1" || col.collider.tag == "Player2")
        {
            col.gameObject.GetComponent<Health>().health += healthAmount;
            GameObject particle = ObjectPooler.GetPooledObject(particleOnCollect);
            particle.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }
}
