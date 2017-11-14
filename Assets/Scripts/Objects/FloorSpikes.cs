using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SpikeType
{
    Constantly,
    OnContact
}

public class FloorSpikes : MonoBehaviour {

    public SpikeType spikeType;

    private Animator animator;


	// Use this for initialization
	void Start () 
    {
        animator = GetComponentInChildren<Animator>();
	}

    IEnumerator boolCooldown()
    {
        animator.SetBool("Trigger", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Trigger", false);
    }

    void FixedUpdate()
    {
        if (spikeType == SpikeType.Constantly)
            animator.SetBool("Trigger", true);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 14)
            StartCoroutine(boolCooldown());
    }
}
