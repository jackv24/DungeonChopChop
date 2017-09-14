using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPickup : MonoBehaviour
{

    public bool canPickUp;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<PlayerInformation>())
        {
            if (canPickUp)
            {
				Pickup(col.GetComponent<PlayerInformation>());
            }
        }
    }

	void Pickup(PlayerInformation playerInfo)
	{
		PlayerAttack playerAttack = playerInfo.GetComponent<PlayerAttack>();

		playerAttack.AddSword(GetComponent<SwordStats>());
	}
}
