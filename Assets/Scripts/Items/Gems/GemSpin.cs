using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpin : MonoBehaviour {

    public float rotationSpeed;
    [Header("Rotation Axis")]
    [Tooltip("X, Y or Z")]
    public string axis;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (axis != null && axis.Length > 0)
        {
            if (axis == "X" || axis == "x")
                transform.localEulerAngles += new Vector3(rotationSpeed, 0, 0);
            else if (axis == "Y" || axis == "y")
                transform.localEulerAngles += new Vector3(0, rotationSpeed, 0);
            else if (axis == "Z" || axis == "z")
                transform.localEulerAngles += new Vector3(0, 0, rotationSpeed);
        }
	}
}
