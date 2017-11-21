using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpikeReset : MonoBehaviour {

    public float resetSpikeTime = 10;

    public FloorSpikes[] spikes;
    public Lever[] levers;

	// Use this for initialization
	void Start () 
    {
        if (spikes.Length > 0)
            spikes[0].OnSpikeDown += ResetSpikes;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ResetSpikes()
    {
        Debug.Log("did");
        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(resetSpikeTime);

        foreach (FloorSpikes spike in spikes)
            spike.Activate();

        foreach (Lever lever in levers)
            lever.Deactivate();
    }
}
