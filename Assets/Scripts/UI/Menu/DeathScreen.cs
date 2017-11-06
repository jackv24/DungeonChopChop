using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour {

    public GameObject deathScreen;
    private bool allPlayersDead = false;

    private bool deathScreenShown = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        foreach (PlayerInformation player in GameManager.Instance.players)
        {
            if (player.animator)
            {
                if (player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dead"))
                {
                    allPlayersDead = true;
                }
                else
                {
                    allPlayersDead = false;
                    break;
                }
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
        deathScreen.SetActive(true);

        WheelOfFortune wheel = GameObject.FindObjectOfType<WheelOfFortune>();
        wheel.SpinWheel();
    }
}
