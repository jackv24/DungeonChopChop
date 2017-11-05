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
    [Tooltip("Chance of this challenge happening")]
    public float chance = 100;
    public bool randomChallenge = false;

    [Header("Colours")]
    public Color completedColor;
    public Color failedColor;

    [Header("Sounds")]
    public SoundEffect completedSound;
    public SoundEffect failedSound;

    private LevelTile levelTile;
    private EnemySpawner enemySpawner;
    private Text cT;

    [Header("Kill Enemies Values")]
    public string killEnemyText;
    public float timeToKillAll;
    private int counter = 0;

    [Header("Take No Damage Values")]
    public string TakeNoDamageText;
    private bool tookDamage = false;

    private bool completed = false;
    private bool questActive = true;
    private bool questStarted = false;

    private bool failed = false;

    private Color originalColor;

	// Use this for initialization
	void Start () 
    {
        GameObject t = GameObject.FindGameObjectWithTag("ChallengeText");
        if (t)
            cT = t.GetComponent<Text>();
        
        if (cT)
        {
            cT.enabled = false;
            originalColor = cT.color;
        }

        foreach (PlayerInformation player in GameManager.Instance.players)
            player.GetComponent<Health>().OnHealthChange += TookDamage;

        levelTile = GetComponentInParent<LevelTile>();

        if (levelTile)
        {
            levelTile.OnTileEnter += StartChallenge;
            levelTile.OnTileExit += ChallengeDisabled;
        }

        enemySpawner = GetComponent<EnemySpawner>();

        enemySpawner.OnEnemiesDefeated += EnemiesKilled;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (questActive)
        {
            if (questStarted)
            {
                if (challengeType == ChallengeType.KillEnemiesInTime)
                {
                    //counts up to the time to kill all
                    counter++;

                    //ran out of time
                    if (!failed)
                    {
                        if (counter > timeToKillAll * 60)
                        {
                            completed = false;
                            failed = true;
                            FailedQuest();
                        }
                    }
                }
            }
        }
	}

    void SetText()
    {
        cT.gameObject.SetActive(false);
        cT.gameObject.SetActive(true);

        cT.color = originalColor;

        cT.enabled = true;

        if (challengeType == ChallengeType.KillEnemiesInTime)
            cT.text = killEnemyText;
        else if (challengeType == ChallengeType.TakeNoDamage)
            cT.text = TakeNoDamageText;
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

                SetText();

                questStarted = true;
            }
        }
    }

    void TookDamage()
    {
        if (questActive)
        {
            if (questStarted)
            {
                if (challengeType == ChallengeType.TakeNoDamage)
                {
                    tookDamage = true;
                    FailedQuest();
                }
            }
        }
    }

    void EnemiesKilled()
    {
        if (challengeType == ChallengeType.KillEnemiesInTime)
        {
            //all the enemies were killed in time, therefore challenge completed
            if (counter < timeToKillAll * 60)
                CompletedQuest();

        }
        else if (challengeType == ChallengeType.TakeNoDamage)
        {
            //the player did not take any damage
            if (!tookDamage)
                CompletedQuest();
        }
    }

    void CompletedQuest()
    {
        Debug.Log("QUEST COMPLETED");

        //set the colour to the correct color
        cT.color = completedColor;

        //play sound
        SoundManager.PlaySound(completedSound, transform.position);
    }

    void FailedQuest()
    {
        Debug.Log("QUEST FAILED");

        //set the colour to the correct color
        cT.color = failedColor;

        //play sound
        SoundManager.PlaySound(failedSound, transform.position);
    }
}
