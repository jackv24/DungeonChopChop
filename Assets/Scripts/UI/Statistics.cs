using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour {

    public static Statistics Instance;

    public float TotalPlayTime;

    [Header("Enemies Killed")]
    public int slimes;
    public int appleShooters;
    public int bisps;
    public int crystalTurrets;
    public int paddyBums;
    public int visps;
    public int snapEyes;
    public int mushBooms;
    [Space()]
    public int totalEnemiesKilled;

    [Header("Health Stats")]
    public float totalDamageTaken;
    public float totalDamageGiven;

    [Header("Other Stats")]
    public float distanceTraveled;
    public int moneyEarned;

    private float timeRemover;

	// Use this for initialization
	void Start () {

        timeRemover = Time.time;
        
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {

        TotalPlayTime = Time.time - timeRemover;

        totalEnemiesKilled = slimes + appleShooters + bisps + crystalTurrets + paddyBums + visps + snapEyes + mushBooms;

	}

    public void GetEnemy(EnemyType enemyType)
    {
        if (enemyType == EnemyType.Slime)
            slimes++;
        else if (enemyType == EnemyType.Bisp)
            bisps++;
        else if (enemyType == EnemyType.Visp)
            visps++;
        else if (enemyType == EnemyType.PaddyBum)
            paddyBums++;
        else if (enemyType == EnemyType.AppleShooter)
            appleShooters++;
        else if (enemyType == EnemyType.CrystalTurret)
            crystalTurrets++;
        else if (enemyType == EnemyType.SnapEye)
            snapEyes++;
        else if (enemyType == EnemyType.MushBoom)
            mushBooms++;
    }
}
