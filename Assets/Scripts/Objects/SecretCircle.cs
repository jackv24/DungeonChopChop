﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretCircle : MonoBehaviour
{

    private Drops[] drops;
    public bool didDrops = false;
    public GameObject[] flowers;

    void OnEnable()
    {
        ReEnableCircle();
    }

    void ReEnableCircle()
    {
        if (flowers.Length > 0)
        {
            foreach (GameObject flower in flowers)
            {
                flower.SetActive(true);
            }
        }

        didDrops = false;
    }

    // Use this for initialization
    void Start()
    {
        drops = GetComponentsInChildren<Drops>();
    }

    void DoDrops()
    {
        if (!didDrops)
        {
            if (drops.Length > 0)
            {
                //goes through each spawn point and spawns the drop
                foreach (Drops drop in drops)
                {
                    drop.DoDrop();
                    SpawnEffects.EffectOnHit(drop.GetComponent<SecretCircleParticles>().particles, drop.transform.position);
                }

                if (flowers.Length > 0)
                {
                    foreach (GameObject flower in flowers)
                    {
                        flower.SetActive(false);
                    }
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
                //gets the frame of the animation
                int currentFrame = ((int)(col.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime * (17))) % 17;

                //checks to make sure its inbetween these values, otherwise you can run into the secret circle at any point in time of animation
                if (currentFrame > 6 && currentFrame < 8)
                {
                    if (!didDrops)
                        DoDrops();
                }
            }
        }
    }
}
