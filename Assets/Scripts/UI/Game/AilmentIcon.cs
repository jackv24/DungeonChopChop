using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AilmentIcon : MonoBehaviour {

    public GameObject host;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (host)
        {
            if (host.activeSelf)
                transform.position = new Vector3(host.transform.position.x, transform.position.y, host.transform.position.z);
            else
                gameObject.SetActive(false);
        }
	}
}
