using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour {

	public int coinAmount;
    public float floatStrength = .5f; 
    public float rotationStrength;
    public float maxDistanceFromSpawn = 2;
    public float speed = 5;

    private float randomRotationStrength;
    private float distance;
    private Vector3 ogPosition;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        transform.parent = null;
        ogPosition = transform.position;
    }

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player1" || col.tag == "Player2") {
            ItemsManager.Instance.Coins += coinAmount;
			gameObject.SetActive (false);
		}
	}

    void FixedUpdate()
    {
        //making coin float


//        distance = Vector3.Distance(transform.position, ogPosition);
//        Debug.Log(distance);
//        if (distance < maxDistanceFromSpawn)
//        {
//            randomRotationStrength = Random.Range(0, rotationStrength);
//            rb.AddForce(Vector3.up * floatStrength);
//            transform.Rotate(randomRotationStrength, randomRotationStrength, randomRotationStrength);
//        }
//        else
//        {
//            transform.position = Vector3.MoveTowards(transform.position, ogPosition, speed * Time.deltaTime);
//        }
    }

}
