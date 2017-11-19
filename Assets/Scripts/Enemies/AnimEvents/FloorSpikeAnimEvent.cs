using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikeAnimEvent : MonoBehaviour {

    private FloorSpikes floorSpike;

	// Use this for initialization
	void Start () 
    {
        floorSpike = GetComponentInParent<FloorSpikes>();	
	}
	
    public void PlaySound()
    {
        SoundManager.PlaySound(floorSpike.soundOnUp, transform.position);
        SpawnEffects.EffectOnHit(floorSpike.particleOnUp, transform.position);
    }
}
