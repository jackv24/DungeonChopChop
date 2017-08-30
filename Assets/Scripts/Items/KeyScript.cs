using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour {

    public float floatSpeed = 1.0f;
    public float rotateSpeed = 5;
    public float floatMagnitude = 1.0f;

    private bool doFloat = false;
    private Rigidbody rb;

    private Vector3 initialPos;
    private Vector3 rotation;

    private float distToGround;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }


    void OnTriggerEnter(Collider col)
    {
        //checks if the player collides with the item
        if (col.tag == "Player1" || col.tag == "Player2") {
            ItemsManager.Instance.Keys += 1 * (int)col.GetComponent<PlayerInformation>().GetCharmFloat("keyMultiplier");
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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 1000))
        {
            initialPos = hit.point;
        }
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

        if (transform.position.y > 5)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0, transform.position.z), 10 * Time.deltaTime);
        }
    }
}
