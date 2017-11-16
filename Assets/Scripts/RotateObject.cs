using UnityEngine;
using System.Collections;

[AddComponentMenu("AIE Scripts/GameDesignFoundations/Rotate Object")]

public class RotateObject : MonoBehaviour {

    public int XRotation;
    public int YRotation;
    public int ZRotation;
    public bool rotateInWorldSpace = false;


	void Update () {
        if (rotateInWorldSpace)
        {
            transform.RotateAround(transform.position, Vector3.right, XRotation * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.up, YRotation * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.forward, ZRotation * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(transform.position, transform.right, XRotation * Time.deltaTime);
            transform.RotateAround(transform.position, transform.up, YRotation * Time.deltaTime);
            transform.RotateAround(transform.position, transform.forward, ZRotation * Time.deltaTime);
        }
    }
	

}
