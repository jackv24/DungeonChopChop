using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVispDeathChecker : MonoBehaviour {

    private BossVispAttack bossVisp;
    private GameObject child;

	// Use this for initialization
	void Start () 
    {
        bossVisp = transform.GetComponentInChildren<BossVispAttack>();
        child = transform.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!child.gameObject.activeSelf)
        {
            foreach (VispAttack enemy in bossVisp.spawnedVisps)
                enemy.GetComponent<Health>().AffectHealth(-100);
            
            gameObject.SetActive(false);
        }
	}
}
