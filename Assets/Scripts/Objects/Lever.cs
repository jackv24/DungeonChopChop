using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

    public delegate void LeverEvent();

    public event LeverEvent OnLeverActivated;
    public event LeverEvent OnLeverDisabled;

    [Header("Audio and Particles")]
    public SoundEffect soundOnHit;
    public AmountOfParticleTypes[] particleOnHit;

	public bool canBeDisabled = true;

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
                animator.SetBool("Trigger", true);

                if (OnLeverActivated != null)
                    OnLeverActivated();

                m_activated = true;
            }
            else
            {
				if (canBeDisabled) {
					animator.SetBool ("Trigger", false);

					if (OnLeverDisabled != null)
						OnLeverDisabled ();

					m_activated = false;
				}
            }

            SoundManager.PlaySound(soundOnHit, transform.position);
            SpawnEffects.EffectOnHit(particleOnHit, transform.position);
        }
    }

    public void Deactivate()
    {
        animator.SetBool("Trigger", false);

        if (OnLeverDisabled != null)
            OnLeverDisabled();

        m_activated = false;
    }
}
