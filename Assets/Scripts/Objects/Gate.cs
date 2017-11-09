using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

    public bool open = false;
    public bool requiresKey = true;

    [Space()]
    public bool requiresChallenge = false;

    private TileQuest tileQuest;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        tileQuest = GetComponentInParent<TileQuest>();

        if (requiresChallenge)
            tileQuest.OnQuestComplete += OpenGate;
    }


    void OnCollisionEnter(Collider col)
    {
        if (!open)
        {
            if (!requiresChallenge)
            {
                //get player layer
                if (col.gameObject.layer == 14)
                {
                    if (requiresKey)
                    {
                        if (ItemsManager.Instance.Keys > 0)
                        {
                            ItemsManager.Instance.Keys--;
                            OpenGate();
                        }
                    }
                    else
                        OpenGate();
                }
            }
        }
    }

    void OpenGate()
    {
        open = true;

        if (animator)
            animator.SetTrigger("Open");
    }
}
