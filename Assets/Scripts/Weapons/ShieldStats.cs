using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldStats : MonoBehaviour {

    public string shieldName;
	[Header("Shield values")]
    public float blockingResistance;
    [Tooltip("0 = not move, 1 = move at normal speed")]
	public float speedDamping = .5f; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
