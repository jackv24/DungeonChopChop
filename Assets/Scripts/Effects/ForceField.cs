using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour {

    private MeshRenderer rend;
    public float xIncrease;
    public float yIncrease;

	// Use this for initialization
	void Start () {
        rend = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rend.material.mainTextureOffset = new Vector2(rend.material.mainTextureOffset.x + xIncrease, rend.material.mainTextureOffset.y + yIncrease);
	}
}
