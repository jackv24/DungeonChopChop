using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableItems : MonoBehaviour {

    [HideInInspector]
    public bool canPickup = false;
    [Tooltip("Time till the player can pick up the coin")]
    public float pickupDelay = 1f;
    [Tooltip("Hit == When it spawns, Death == When picked up")]
    public AmountOfParticleTypes[] particles;

    protected SpawnEffects spawnEffects;

    protected void DidEnable()
    {
        spawnEffects = GameObject.FindObjectOfType<SpawnEffects>();
    }

    protected void DoSpawnParticle(Vector3 position)
    {
        spawnEffects.EffectOnHit(particles, position);
    }

}
