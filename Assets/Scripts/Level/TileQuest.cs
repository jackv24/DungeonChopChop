using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ChallengeType
{
    KillEnemiesInTime,
    TakeNoDamage,
    LeversInTime,
}

[System.Serializable]
public class TileQuest : MonoBehaviour {

    public delegate void TileEvent();

    public event TileEvent OnQuestStarted;
    public event TileEvent OnQuestComplete;
    public event TileEvent OnQuestFailed;

    public ChallengeType challengeType;
    [Tooltip("Chance of this challenge happening")]
    public float chance = 100;
    public bool randomChallenge = false;

    public string tileQuestText;

    [Header("Colours")]
    public Color completedColor;
    public Color failedColor;

    [Header("Sounds")]
    public SoundEffect completedSound;
    public SoundEffect failedSound;

    private LevelTile levelTile;
    private EnemySpawner enemySpawner;
    private Text cT;

    [Space()]

    [HideInInspector]
    public float timeToDoTask;

    //Kill Enemies Values
    private int counter = 0;

    //Take No Damage Values
    private bool tookDamage = false;

    //Levers in time Values
    [Header("Lever Values")]
    public Lever[] levers;
    private int leverCounter = 0;

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
            player.GetComponent<Health>().OnHealthNegative += TookDamage;

        levelTile = GetComponentInParent<LevelTile>();

        if (levelTile)
        {
            levelTile.OnTileEnter += StartChallenge;
            levelTile.OnTileExit += ChallengeDisabled;
        }

        enemySpawner = GetComponent<EnemySpawner>();

        enemySpawner.OnEnemiesDefeated += EnemiesKilled;

        //if the challenge is levers, subscribe to each levers on lever activated
        if (challengeType == ChallengeType.LeversInTime)
        {
            foreach (Lever lever in levers)
            {
                lever.OnLeverActivated += CheckLeversTriggered;
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (questActive)
        {
            if (questStarted)
            {
                if (challengeType == ChallengeType.KillEnemiesInTime || challengeType == ChallengeType.LeversInTime)
                {
                    //counts up to the time to kill all
                    counter++;

                    //ran out of time
                    if (!failed)
                    {
                        if (counter > timeToDoTask * 60)
                        {
                            FailedQuest();
                        }

                        cT.text = tileQuestText + "\n" + (int)(timeToDoTask - (counter / 60));
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

        cT.text = tileQuestText;
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

            if (c <= chance || chance == 100)
            {
                if (randomChallenge)
                    RandomChallenge();

                SetText();

                if (OnQuestStarted != null)
                    OnQuestStarted();

                questStarted = true;
            }
        }
    }

    bool CompletedInTime()
    {
        if (counter < timeToDoTask * 60)
            return true;
        return false;
    }

    void CheckLeversTriggered()
    {
        leverCounter++;
        if (leverCounter >= levers.Length)
        {
            if (CompletedInTime())
                CompletedQuest();
            else
                FailedQuest();
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
            if (CompletedInTime())
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

        questActive = false;
        completed = true;
        failed = false;

        //play sound
        SoundManager.PlaySound(completedSound, transform.position);

        if (OnQuestComplete != null)
            OnQuestComplete();
    }

    void FailedQuest()
    {
        Debug.Log("QUEST FAILED");

        //set the colour to the correct color
        cT.color = failedColor;

        questActive = false;
        completed = false;
        failed = true;

        //play sound
        SoundManager.PlaySound(failedSound, transform.position);

        if (OnQuestFailed != null)
            OnQuestFailed();
    }
}
