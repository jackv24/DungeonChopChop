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

    [HideInInspector]
    public Animator animator;

    private LevelTile tile;
    private EnemySpawner enemySpawner;

    private bool startSpiking = false;
    private EnemyAttack enemyAttack;

    private bool active = false;
    private int leverCounter = 0;
    private bool spiking = false;

    private bool firstEnter = true;

    private bool overrideActive = true;


	// Use this for initialization
	void Start () 
    {
        animator = GetComponentInChildren<Animator>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();

        if (enemyAttack)
            enemyAttack.damagesOtherEnemies = true;

        tile = GetComponentInParent<LevelTile>();
        enemySpawner = GetComponentInParent<EnemySpawner>();

        if (tile)
        {
            tile.OnTileEnter += SetActive;
            tile.OnTileExit += SetDeactive;
        }

        if (enemySpawner)
            enemySpawner.OnEnemiesDefeated += Deactivate;

        StartCoroutine(spikeDelay());

        SetupLevers();

        animator.speed = animationSpeed;
	}

    void SetupLevers()
    {
		if (tile)
        	levers = tile.GetComponentsInChildren<Lever>();

        if (levers.Length > 0)
        {
			foreach (Lever lever in levers) {
				if (lever)
					lever.OnLeverActivated += Deactivated;
			}
        }
    }

    void SetActive()
    {
        active = true;

		if (animator) {
			animator.gameObject.SetActive (true);

			if (startUp)
				animator.SetBool ("Trigger", true);
		}
    }

    void SetDeactive()
    {
        active = false;
        animator.gameObject.SetActive(false);
    }

    IEnumerator spikeDelay()
    {
        yield return new WaitForSeconds(delayTime);
        startSpiking = true;
    }

    IEnumerator boolCooldown()
    {
        animator.SetBool("Trigger", true);
        if (OnSpikeUp != null)
            OnSpikeUp();
        yield return new WaitForEndOfFrame();
        animator.SetBool("Trigger", false);
        if (OnSpikeDown != null)
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
        active = false;
        overrideActive = false;
    }

    public void Activate()
    {
        animator.SetBool("Trigger", true);
        active = true;
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
        if (active && overrideActive)
        {
            if (animator.gameObject.activeSelf)
            {
                if (startSpiking)
                {
                    if (spikeType == SpikeType.Constantly)
                    {
                        animator.SetBool("Trigger", true);
                        spiking = true;
                    }
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
