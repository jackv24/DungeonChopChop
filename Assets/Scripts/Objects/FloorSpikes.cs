using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SpikeType
{
    Constantly,
    OnContact,
    UsesLever,
}

public class FloorSpikes : MonoBehaviour {

    public delegate void SpikeEvent();

    public event SpikeEvent OnSpikeUp;
    public event SpikeEvent OnSpikeDown;

    public SpikeType spikeType;
    public float delayTime = 0;
    public float animationSpeed = 1;
    public bool startUp;

    public Lever[] levers;

    [Header("Audio and Particles")]
    public AmountOfParticleTypes[] particleOnUp;
    public SoundEffect soundOnUp;

    private LevelTile tile;
    [HideInInspector]
    public Animator animator;
    private bool startSpiking = false;
    private EnemyAttack enemyAttack;

    private bool active = false;
    private int leverCounter = 0;


	// Use this for initialization
	void Start () 
    {
        enemyAttack = GetComponentInChildren<EnemyAttack>();

        if (enemyAttack)
            enemyAttack.damagesOtherEnemies = true;

        tile = GetComponentInParent<LevelTile>();

        if (tile)
        {
            tile.OnTileEnter += SetState;
            tile.OnTileExit += SetState;
        }

        animator = GetComponentInChildren<Animator>();
        StartCoroutine(spikeDelay());

        animator.speed = animationSpeed;

        if (levers.Length > 0)
        {
            foreach(Lever lever in levers)
                lever.OnLeverActivated += Deactivated;
        }

        if (startUp)
            animator.SetBool("Trigger", true);
	}

    void SetState()
    {
        if (active)
            active = false;
        else
            active = true;
    }

    IEnumerator spikeDelay()
    {
        yield return new WaitForSeconds(delayTime);
        startSpiking = true;
    }

    IEnumerator boolCooldown()
    {
        animator.SetBool("Trigger", true);
        OnSpikeUp();
        yield return new WaitForEndOfFrame();
        animator.SetBool("Trigger", false);
        OnSpikeDown();
    }

    public void SpikeUp()
    {
        if (OnSpikeUp != null)
            OnSpikeUp();
    }

    public void SpikeDown()
    {
        if (OnSpikeDown != null)
            OnSpikeDown();
    }

    public void Deactivate()
    {
        animator.SetBool("Trigger", false);
    }

    public void Activate()
    {
        animator.SetBool("Trigger", true);
    }

    void Deactivated()
    {
        leverCounter++;

        if (leverCounter >= levers.Length)
        {
            animator.SetBool("Trigger", false);
            SpikeDown();
            leverCounter = 0;
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            if (animator.gameObject.activeSelf)
            {
                if (startSpiking)
                {
                    if (spikeType == SpikeType.Constantly)
                        animator.SetBool("Trigger", true);
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (spikeType == SpikeType.OnContact)
        {
            if (col.gameObject.layer == 14)
                StartCoroutine(boolCooldown());
        }
    }
}
