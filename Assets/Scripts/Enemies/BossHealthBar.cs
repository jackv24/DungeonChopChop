using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour {

    public static BossHealthBar Instance;

    private Image bossBar;
    private Health bossHealth;
    private float m_maxHealth;

	// Use this for initialization
	void Start () 
    {
        Instance = this;

        bossBar = GetComponent<Image>();

        bossBar.transform.parent.gameObject.SetActive(false);
	}
	
    void UpdateBar()
    {
        bossBar.fillAmount = bossHealth.health / m_maxHealth;
    }

    void OnDeath()
    {
        //unsubscribe
        bossHealth.OnHealthChange -= UpdateBar;
        bossHealth.OnDeath -= OnDeath;

        bossBar.transform.parent.gameObject.SetActive(false);
    }

    public void SetBoss(Health boss, float maxHealth = 0)
    {
        bossBar.transform.parent.gameObject.SetActive(true);

        bossHealth = boss;

        if (maxHealth == 0)
            m_maxHealth = bossHealth.maxHealth;
        else
            m_maxHealth = maxHealth;

        bossHealth.OnHealthChange += UpdateBar;

        bossHealth.OnDeath += OnDeath;
    }

    public void Disable()
    {
        bossBar.transform.parent.gameObject.SetActive(false);
    }
}
