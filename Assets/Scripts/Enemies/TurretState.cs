using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretState : MonoBehaviour {

    public bool active = true;
    public Lever lever;

    private bool overrideActive = true;
    private EnemyAttack enemyAttack;
    private LevelTile tile;
    private EnemySpawner enemySpawner;
    private Animator animator;

	// Use this for initialization
	void Start () 
    {
        animator = GetComponentInChildren<Animator>();

        enemySpawner = GetComponentInParent<EnemySpawner>();

        enemySpawner.OnEnemiesDefeated += Deactivate;
        
        tile = GetComponentInParent<LevelTile>();
        enemyAttack = GetComponent<EnemyAttack>();

        if (tile)
        {
            tile.OnTileEnter += SetState;
            tile.OnTileExit += SetState;
        }

        if (lever)
            lever.OnLeverActivated += SetState;
	}

    void Deactivate()
    {
        overrideActive = false;
        animator.SetBool("Active", false);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (overrideActive)
        {
            if (active)
                enemyAttack.enabled = true;
            else
                enemyAttack.enabled = false;
        }
	}

    void SetState()
    {
        if (overrideActive)
        {
            if (active)
                active = false;
            else
                active = true;
        }
    }
}
