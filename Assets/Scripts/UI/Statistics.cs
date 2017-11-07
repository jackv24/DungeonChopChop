using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyKind
{
    public EnemyType enemyType;
    public EnemyBiomeType enemyBiome;
}

[System.Serializable]
public enum EnemyBiomeType
{
    All,
    Grass,
    Forest,
    Desert,
    Ice,
    Fire,
    Dungeon
}

[System.Serializable]
public enum EnemyType
{
    None,
    Slime,
    AppleShooter,
    Bisp,
    Visp,
    SnapEye,
    CrystalTurret,
    PaddyBum,
    MushBoom,
    Snowballs,
    Fireball
}

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
    public int Snowballs;
    public int Fireballs;
    [Space()]
    public int totalEnemiesKilled;

    public List<EnemyKind> enemiesKilled = new List<EnemyKind>(0);

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

        totalEnemiesKilled = 
            slimes 
            + appleShooters 
            + bisps 
            + crystalTurrets 
            + paddyBums 
            + visps 
            + snapEyes 
            + mushBooms 
            + Snowballs 
            + Fireballs;

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
        else if (enemyType == EnemyType.Snowballs)
            Snowballs++;
        else if (enemyType == EnemyType.Fireball)
            Fireballs++;
    }
}
