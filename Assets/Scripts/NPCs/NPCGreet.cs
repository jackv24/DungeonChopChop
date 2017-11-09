using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGreet : MonoBehaviour {

    public ShopSpawner shopSpawner;

    private Animator animator;

	// Use this for initialization
	void Start () 
    {
        shopSpawner.OnItemPurchased += DoGreeting;	

        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DoGreeting()
    {
        if (animator)
            animator.SetTrigger("Greeting");
    }
}
