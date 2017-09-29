using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum WeaponEffect
{
    Nothing,
    Burn,
    Poison,
    SlowDeath,
}

public class SwordStats : MonoBehaviour {

    public string swordName = "Sword";
    public float damageMultiplier = 1;
    public int range = 1;
    public WeaponEffect weaponEffect;
    public float damagePerTick;
    public float duration;
    public float timeBetweenEffect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
