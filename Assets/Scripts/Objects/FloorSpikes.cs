using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SpikeType
{
    Constantly,
    OnContact,
    UsesLever,
}

public class FloorSpikes : MonoBehaviour {

    public SpikeType spikeType;
    public float delayTime = 0;
    public float animationSpeed = 1;
    public bool startUp;

    public Lever lever;

    private Animator animator;
    private bool startSpiking = false;


	// Use this for initialization
	void Start () 
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(spikeDelay());

        animator.speed = animationSpeed;

        if (lever)
            lever.OnLeverActivated += Deactivated;

        if (startUp)
            animator.SetBool("Trigger", true);
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

    void Deactivated()
    {
        animator.SetBool("Trigger", false);
    }

    void FixedUpdate()
    {
        if (animator.gameObject.activeSelf)
        {
            if (startSpiking)
            {
                if (spikeType == SpikeType.Constantly)
                    animator.SetBool("Trigger", true);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (spikeType == SpikeType.OnContact)
        {
            if (col.gameObject.layer == 14)
                StartCoroutine(boolCooldown());
        }
    }
}
