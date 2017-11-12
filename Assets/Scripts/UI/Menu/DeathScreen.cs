using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeathScreen : MonoBehaviour {

    public GameObject deathScreen;
    public GameObject replayButton;

    private bool allPlayersDead = false;

    public static bool deathScreenShown = false;

	// Use this for initialization
	void Start () {
		
	}

    void OnEnable()
    {
        deathScreenShown = false;
    }
	
	// Update is called once per frame
	void Update () {
        
        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            if (player.playerMove.playerHealth.health <= 0)
            {
                allPlayersDead = true;
            }
            else
            {
                allPlayersDead = false;
                break;
            }
        }

        if (allPlayersDead)
        {
            if (!deathScreenShown)
            {
                StartCoroutine(deathScreenWait());
                deathScreenShown = true;
            }
        }
	}

    IEnumerator deathScreenWait()
    {
        yield return new WaitForSeconds(2);

        EventSystem.current.SetSelectedGameObject(replayButton);

        EventSystem.current.firstSelectedGameObject = replayButton;

        deathScreen.SetActive(true);

        WheelOfFortune wheel = GameObject.FindObjectOfType<WheelOfFortune>();
        wheel.SpinWheel();

        TotalEnemiesKilled enemyIconSpawner = GameObject.FindObjectOfType<TotalEnemiesKilled>();

        StartCoroutine(enemyIconSpawner.SpawnEnemyIcons());
    }
}
