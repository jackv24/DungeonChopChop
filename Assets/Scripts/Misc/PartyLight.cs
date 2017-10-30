using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyLight : MonoBehaviour {

    [HideInInspector]
    public float lightOnAndOffTime;

    private Color col;
    private Light l;

    private int counter = 0;
    private float randomTime;

	// Use this for initialization
	void Start () 
    {
        l = GetComponent<Light>();	
	}

    void OnEnable()
    {
        if (l)
            l.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        randomTime = Random.Range(0, lightOnAndOffTime);
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        counter++;

        if (counter > randomTime * 60)
        {
            if (l.enabled)
                l.enabled = false;
            else
                l.enabled = true;

            counter = 0;
           
            randomTime = Random.Range(0, lightOnAndOffTime);
        }
	}
}
