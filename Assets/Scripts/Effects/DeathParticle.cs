using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticle : MonoBehaviour {

    public float timeBeforeDisabled;

	// Use this for initialization
	void OnEnable () {
        StartCoroutine(destroyWait());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator destroyWait()
    {
        yield return new WaitForSeconds(timeBeforeDisabled);
        gameObject.SetActive(false);
    }
}
