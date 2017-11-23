using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBossDeath : MonoBehaviour {

    public float waitTillDeathScreen = 3;

    private Health bossHealth;
    private DeathScreen deathScreen;

	// Use this for initialization
	void Start () 
    {
        bossHealth = GetComponent<Health>();
        deathScreen = FindObjectOfType<DeathScreen>();

        if (bossHealth)
            bossHealth.OnDeath += DoDeath;
	}
	
    void DoDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(waitTillDeathScreen);

        deathScreen.DoDeathScreen();
    }
}
