using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushAnim : MonoBehaviour {

    MushBoomAttack mushAttack;

	// Use this for initialization
	void Start () {
        mushAttack = GetComponentInParent<MushBoomAttack>();
	}

	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoBoom()
    {
        mushAttack.Boom();
    }
}
