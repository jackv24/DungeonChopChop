using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ChallengeType
{
    KillEnemiesInTime,
    TakeNoDamage,
}

public class TileQuest : MonoBehaviour {

    public ChallengeType challengeType;
    public string challengeText;
    [Tooltip("Chance of this challenge happening")]
    public float chance = 100;
    public bool randomChallenge = false;

    private LevelTile levelTile;
    private EnemySpawner enemySpawner;
    private Text cT;

    [Header("Kill Enemies Values")]
    public float timeToKillAll;
    private int counter = 0;


    private bool completed = false;
    private bool questActive = true;

	// Use this for initialization
	void Start () 
    {
        GameObject t = GameObject.FindGameObjectWithTag("ChallengeText");
        if (t)
            cT = t.GetComponent<Text>();


        levelTile = GetComponentInParent<LevelTile>();

        if (levelTile)
        {
            levelTile.OnTileEnter += StartChallenge;
            levelTile.OnTileExit += ChallengeDisabled;
        }

        enemySpawner = GetComponent<EnemySpawner>();

        if (enemySpawner)
            enemySpawner.OnEnemiesDefeated += EnemiesKilled;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (questActive)
        {
            if (challengeType == ChallengeType.KillEnemiesInTime)
            {
                //counts up to the time to kill all
                counter++;

                //ran out of time
                if (counter > timeToKillAll * 60)
                {
                    completed = false;
                }
            }
        }
	}

    void SetText()
    {
        cT.enabled = true;
        cT.text = challengeText;
    }

    void ChallengeDisabled()
    {
        questActive = false;
        cT.enabled = false;
    }

    void RandomChallenge()
    {
        int random = Random.Range(0, System.Enum.GetNames(typeof(ChallengeType)).Length);

        if (random == 0)
            challengeType = ChallengeType.KillEnemiesInTime;
        else if (random == 1)
            challengeType = ChallengeType.TakeNoDamage;
    }

    void StartChallenge()
    {
        //checks to make sure the challenge can be done
        if (questActive)
        {
            float c = Random.Range(0, 101);

            if (c <= chance)
            {
                if (randomChallenge)
                    RandomChallenge();

                //gets the challenge
                if (challengeType == ChallengeType.TakeNoDamage)
                    StartCoroutine(TakeNoDamage());

                SetText();
            }
        }
    }

    void EnemiesKilled()
    {
        if (challengeType == ChallengeType.KillEnemiesInTime)
        {
            //all the enemies were killed in time, therefore challenge completed
            if (counter < timeToKillAll * 60)
                completed = true;

            if (completed)
                CompletedQuest();
        }
    }

    IEnumerator KillEnemiesInTime()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator TakeNoDamage()
    {
        yield return new WaitForSeconds(1);
    }

    void CompletedQuest()
    {
        Debug.Log("QUEST COMPLETED");
    }
}
