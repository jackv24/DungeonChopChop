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
    public float delayTime = 0;
    public float animationSpeed = 1;

    private Animator animator;
    private bool startSpiking = false;


	// Use this for initialization
	void Start () 
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(spikeDelay());

        animator.speed = animationSpeed;
	}

    IEnumerator spikeDelay()
    {
        yield return new WaitForSeconds(delayTime);
        startSpiking = true;
    }

    IEnumerator boolCooldown()
    {
        animator.SetBool("Trigger", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Trigger", false);
    }

    void FixedUpdate()
    {
        if (startSpiking)
        {
            if (spikeType == SpikeType.Constantly)
                animator.SetBool("Trigger", true);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 14)
            StartCoroutine(boolCooldown());
    }
}
