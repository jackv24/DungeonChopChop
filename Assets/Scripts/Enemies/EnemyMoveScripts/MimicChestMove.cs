using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicChestMove : EnemyMove {

    public bool opened = false;

	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (opened)
        {
            FollowPlayer();
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (!opened)
        {
            if (col.collider.gameObject.layer == 14)
            {
                if (ItemsManager.Instance)
                {
                    if (ItemsManager.Instance.Keys > 0)
                    {
                        Open();
                        ItemsManager.Instance.Keys -= 1;
                    }
                }
            }
        }
    }

    void Open()
    {
        //opens chest and plays animation
        animator.SetTrigger("Open");
        opened = true;
    }
}
