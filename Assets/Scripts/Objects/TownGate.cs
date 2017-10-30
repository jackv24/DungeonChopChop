using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownGate : MonoBehaviour {

    [Tooltip("The distance the player has to be from the door till the locks drop off")]
    public float distanceTillUnlock;
    [Tooltip("The particle that happens when the lock is unlocked")]
    public AmountOfParticleTypes[] particles;

    public GameObject[] locks;

    private PlayerInformation player;

	// Use this for initialization
	void Start () {
        player = GameManager.Instance.players[0];
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        float dist = Vector3.Distance(new Vector3(-1, 0, 35), player.transform.position);

        if (dist <= distanceTillUnlock)
        {
            if (ItemsManager.Instance.hasGoggles)
                BreakLock(0);
            if (ItemsManager.Instance.hasBoots)
                BreakLock(1);
            if (ItemsManager.Instance.hasArmourPiece)
                BreakLock(2);
            if (ItemsManager.Instance.hasGauntles)
                BreakLock(3);
                
        }
	}

    void BreakLock(int lockNumber)
    {
        if (locks[lockNumber].activeSelf)
        {
            locks[lockNumber].SetActive(false);
            SpawnEffects.EffectOnHit(particles, locks[lockNumber].transform.position); 
        }
    }
}
