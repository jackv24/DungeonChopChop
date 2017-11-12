using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

    public bool requiresKey = true;

    [Space()]
    public bool requiresChallenge = false;

    [Space()] 
    public Lever lever;

    private TileQuest tileQuest;
    private Animator animator;

    private bool open = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        tileQuest = GetComponentInParent<TileQuest>();

        if (requiresChallenge)
            tileQuest.OnQuestComplete += OpenGate;

        if (lever)
            lever.OnLeverActivated += OpenGate;
    }


    void OnCollisionEnter(Collision col)
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
                            ItemsManager.Instance.KeyChange();

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
