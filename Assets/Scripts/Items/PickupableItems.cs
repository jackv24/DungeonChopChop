using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableItems : MonoBehaviour {

    [HideInInspector]
    public bool canPickup = false;
    [Tooltip("Time till the player can pick up the coin")]
    public float pickupDelay = 1f;
    public bool doesDespawn = true;
    public float DespawnDelay = 10;
    [Tooltip("Hit == When it spawns, Death == When picked up")]
    public AmountOfParticleTypes[] particles;

    protected SpawnEffects spawnEffects;

    private Renderer[] renderers;

    protected void DidEnable()
    {
        spawnEffects = GameObject.FindObjectOfType<SpawnEffects>();
        if (doesDespawn)
            StartCoroutine(WaitToDestroy());

        renderers = GetComponentsInChildren<Renderer>();
    }

    protected void DoSpawnParticle(Vector3 position)
    {
        spawnEffects.EffectOnHit(particles, position);
    }

    void DisableRenderers()
    {
        if (renderers != null)
        {
            foreach (Renderer ren in renderers)
            {
                ren.enabled = false;
            }
        }
    }

    void EnableRenderers()
    {
        if (renderers != null)
        {
            foreach (Renderer ren in renderers)
            {
                ren.enabled = true;
            }
        }
    }

    IEnumerator flash(int flashAmount, float timeBetween)
    {
        for (int i = 0; i < flashAmount; i++)
        {
        DisableRenderers();
        yield return new WaitForSeconds(timeBetween);
        EnableRenderers();
        yield return new WaitForSeconds(timeBetween);
        }
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(DespawnDelay);
        StartCoroutine(flash(6, .2f));
        yield return new WaitForSeconds(.2f * 12);
        StartCoroutine(flash(10, .1f));
        yield return new WaitForSeconds(.1f * 20);
        spawnEffects.GetEffectOnDeath(particles);
        gameObject.SetActive(false);
    }
}
