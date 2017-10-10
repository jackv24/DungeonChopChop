using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableItems : MonoBehaviour {

    [HideInInspector]
    public bool canPickup = false;
    [Tooltip("Time till the player can pick up the coin")]
    public float pickupDelay = 1f;

}
