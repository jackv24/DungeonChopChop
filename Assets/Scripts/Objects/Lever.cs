using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

    public delegate void LeverEvent();

    public event LeverEvent OnLeverActivated;

    public bool activated
    {
        get { return m_activated; }
    }

    private bool m_activated;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.gameObject.layer == 16)
        {
            if (!m_activated)
            {
                animator.SetTrigger("Trigger");

                if (OnLeverActivated != null)
                    OnLeverActivated();

                m_activated = true;
            }
        }
    }
}
