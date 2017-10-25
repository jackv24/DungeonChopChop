﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quicksand : MonoBehaviour {

    public bool SarlacsPopUp = false;
    public float moveToCenterSpeed = .7f;
    [Tooltip("The move to center speed when tenticles destroyed")]
    public float speedWhenDestroyed = .2f;
    public float distanceToPopUp = 5;

    [Tooltip("Hit == Pop up")]
    public AmountOfParticleTypes[] popUpParticle;

    private float toCenterSpeed = 0;

    public delegate void NormalEvent();
    public event NormalEvent onSpikesDestroyed;

    private bool spikesDestroyed = false;
    private bool tentsPoppedUp = false;

    private List<GameObject> tenticles = new List<GameObject>();

    private Animator animator;
    private Tenticles tenticleScript;

    void OnEnable()
    {
        toCenterSpeed = moveToCenterSpeed;
        spikesDestroyed = false;
        EnableSpikes();
    }

	// Use this for initialization
	void Start () 
    {
        if (SarlacsPopUp)
        {
            tenticleScript = GetComponentInChildren<Tenticles>();

            //add all the children to a list
            for (int i = 0; i < transform.childCount; i++)
            {
                tenticles.Add(transform.GetChild(i).gameObject);
            }
        
            animator = GetComponent<Animator>();
            onSpikesDestroyed += SetSpeedToCenter;
        }
	}

    public void SpikesHit()
    {
        onSpikesDestroyed();
    }

    void HideSpikes()
    {
        SpawnEffects.EffectOnDeath(tenticleScript.particleTypes, transform.position);
        //loop through all children and enable them
        if (tenticles.Count > 0)
        {
            foreach (GameObject tenticle in tenticles)
            {
                tenticle.gameObject.SetActive(false);
            }
        }
    }

    void EnableSpikes()
    {
        //loop through all children and hide them
        if (tenticles.Count > 0)
        {
            foreach (GameObject tenticle in tenticles)
            {
                tenticle.gameObject.SetActive(true);
            }
        }
    }

    void SetSpeedToCenter()
    {
        if (!spikesDestroyed)
        {
            spikesDestroyed = true;
            toCenterSpeed = speedWhenDestroyed;
            HideSpikes();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player1" || col.gameObject.tag == "Player2")
        {
            col.transform.position = Vector3.Lerp(col.transform.position, transform.position, toCenterSpeed * Time.deltaTime);

            if (SarlacsPopUp)
            {
            //get the distance from player to the center of quicksand
                if (!spikesDestroyed)
                {
                    if (!tentsPoppedUp)
                    {
                        float distance = Vector3.Distance(transform.position, col.transform.position);
                        if (distance <= 5)
                        {
                            tentsPoppedUp = true;
                            animator.SetTrigger("Attack");

                            SpawnEffects.EffectOnHit(popUpParticle, transform.position);

                            StartCoroutine(StopSpikes());
                        }
                    }
                }
            }
        }
    }

    IEnumerator StopSpikes()
    {
        yield return new WaitForSeconds(2);
        tentsPoppedUp = false;
    }
}
