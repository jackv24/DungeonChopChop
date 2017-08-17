using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    public bool opened = false;

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (!opened)
        {
            if (col.collider.gameObject.layer == 14)
            {
                if (ItemsManager.Instance.Keys > 0)
                {
                    animator.SetTrigger("Open");
                    opened = true;
                    ItemsManager.Instance.Keys -= 1;
                }
            }
        }
    }
}
