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
    Ice,
    Sandy
}

public class SwordStats : MonoBehaviour {

    public string swordName = "Sword";
    public float damageMultiplier = 1;
    [Tooltip("Doesn't effect anything, just for showing stat")]
    public int range = 1;
    [Header("Weapon Effect Stuff")]
    public WeaponEffect weaponEffect;
    [HideInInspector] public float damagePerTick;
    [HideInInspector] public float duration;
    [HideInInspector] public float timeBetweenEffect;
    [HideInInspector] public float speedDamper;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
