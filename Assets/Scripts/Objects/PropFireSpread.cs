using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropFireSpread : MonoBehaviour {

    public float spreadRadius;
    [Tooltip("The delay time between setting the closest prop on fire")]
    public float burnDelayTime;
    public LayerMask propLayer;

    private Health propHealth;
    private Collider thisCol;
    private Collider[] cols;

    private bool coroutineInUse = false;

	// Use this for initialization
	void Start () 
    {
        propHealth = GetComponent<Health>();
        thisCol = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (propHealth)
        {
            if (propHealth.isBurned)
            {
                if (!coroutineInUse)
                {
                    StartCoroutine(BurnClosestProps());
                }
            }
        }
	}

    IEnumerator BurnClosestProps()
    {
        coroutineInUse = true;

        cols = Physics.OverlapSphere(transform.position, spreadRadius, propLayer);
        if (cols.Length > 0)
        {
            yield return new WaitForSeconds(burnDelayTime);
            foreach (Collider col in cols)
            {
                if (col)
                {
                    if (col != thisCol)
                    {
                        if (col.GetComponent<Health>())
                        {
                            col.GetComponent<Health>().SetBurned();
                        }
                    }
                }
            }
        }

        coroutineInUse = false;
    }
}
