using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretState : MonoBehaviour {

    public bool active = true;
    public Lever lever;

    private EnemyAttack enemyAttack;
    private LevelTile tile;

	// Use this for initialization
	void Start () 
    {
        if (lever)
            lever.OnLeverActivated += SetState;
        
        tile = GetComponentInParent<LevelTile>();
        enemyAttack = GetComponent<EnemyAttack>();

        if (tile)
        {
            tile.OnTileEnter += SetState;
            tile.OnTileExit += SetState;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (active)
        {
            enemyAttack.enabled = true;
        }
        else
        {
            enemyAttack.enabled = false;
        }
	}

    void SetState()
    {
        if (active)
            active = false;
        else
            active = true;
    }
}
