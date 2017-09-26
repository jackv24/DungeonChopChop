using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quicksand : MonoBehaviour {

    public float moveToCenterSpeed = 5;

	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player1" || col.gameObject.tag == "Player2")
        {
            col.transform.position = Vector3.Lerp(col.transform.position, transform.position, moveToCenterSpeed * Time.deltaTime);
        }
    }
}
