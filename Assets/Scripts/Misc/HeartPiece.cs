using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPiece : PickupableItems {

    void OnEnable()
    {
        DidEnable();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 14)
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.GetComponent<Health>().maxHealth += 1;
                player.GetComponent<Health>().HealthChanged();

                DoPickUpParticle();

                Destroy(gameObject);
            }
        }
    }

    void DoPickUpParticle()
    {
        //do particles
        SpawnEffects.EffectOnDeath(particles, transform.position);

        SoundManager.PlaySound(pickUpSound, transform.position);
    }
}
