using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour {

	public int coinAmount;

    public float floatSpeed = 1.0f;
    public float rotateSpeed = 5;
    public float floatMagnitude = 1.0f;

    private bool doFloat = false;
    private Rigidbody rb;

    private Vector3 initialPos;
    private Vector3 rotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


	void OnTriggerEnter(Collider col)
	{
        //checks if the player collides with the item
		if (col.tag == "Player1" || col.tag == "Player2") {
            ItemsManager.Instance.Coins += coinAmount;
			gameObject.SetActive (false);
		}
	}

    void OnEnable()
    {
        rotation = Random.insideUnitSphere.normalized * rotateSpeed;

        StartCoroutine(wait());
    }

    void OnDisable()
    {
        doFloat = false;
        rb.isKinematic = false;
    }

    IEnumerator wait()
    {
        //waits a second before letting the item to float
        yield return new WaitForSeconds(1);
        initialPos = transform.localPosition;
        rb.isKinematic = true;
        doFloat = true;
    }

    void FixedUpdate()
    {
        //makes item float
        if (doFloat)
        {
            transform.eulerAngles += rotation * Time.fixedDeltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatMagnitude, Time.fixedDeltaTime * floatSpeed);
        }
    }

}
