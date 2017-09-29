using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretCircle : MonoBehaviour {

    private Drops[] drops;
    public bool didDrops = false;

	// Use this for initialization
	void Start () {
        drops = GetComponentsInChildren<Drops>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DoDrops()
    {
        if (!didDrops)
        {
            if (drops.Length > 0)
            {
                foreach (Drops drop in drops)
                {
                    drop.DoDrop();
                }
                didDrops = true;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player1" || col.tag == "Player2")
        {
            if (col.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
            {
                if (!didDrops)
                    DoDrops();
            }
        }
    }
}
