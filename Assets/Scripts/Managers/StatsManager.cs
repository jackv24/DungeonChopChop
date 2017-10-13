using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum StatName
{
	Range,
	Spread,
	RunSpeed,
	AttackSpeed,
	Damage,
	maxHealth,
	Resistance,
	Knockback,
};

[Serializable]
public class Stat
{
	public StatName statName;
	public float[] levelValue;
}

[Serializable]
public class StatLevel
{
    public StatName statName;
    public int level;
}

public class StatsManager : MonoBehaviour 
{
	public Stat[] stats;
    public StatLevel[] playerOneLevel;
    public StatLevel[] playerTwoLevel;

	private PlayerInformation playerOneInfo;
	private Health playerOneHealth;

    private PlayerInformation playerTwoInfo;
    private Health playerTwoHealth;

	private GameObject player1;
    private GameObject player2;

	// Use this for initialization
	void Start () 
	{

	}

	public float GetStatValue(StatName statName, int level)
	{
		foreach (Stat stat in stats) 
		{
			if (stat.statName == statName)
				return stat.levelValue[level - 1];
		}
		return 0;
	}

    public int GetStatLevel(StatName statName, PlayerInformation player)
	{
        //checks if the player info is the same as the player passed through
        if (player == playerOneInfo)
        {
            //loop through each stat and return the level
            foreach (StatLevel stat in playerOneLevel)
            {
                if (stat.statName == statName)
                    return stat.level;
            }
        } 
        else if (player == playerTwoInfo)
        {
            //loop through each stat and return the level
            foreach (StatLevel stat in playerTwoLevel)
            {
                if (stat.statName == statName)
                    return stat.level;
            }
        }
		return 0;
	}

	int GetClosestLevel(StatName statName, float playerStatValue)
	{
		foreach (Stat stat in stats) 
		{
			if (stat.statName == statName) {
				float closestNumber = stat.levelValue [0];
				int valNumber = 0;
				for (int i = 0; i < stat.levelValue.Length; i++) {
					if (Mathf.Abs (stat.levelValue [i] - playerStatValue) < Mathf.Abs (closestNumber - playerStatValue)) {
						closestNumber = stat.levelValue [i];
						valNumber = i;
					}
				}

				return valNumber;
			}
		}
		return 0;
	}
	
	// Update is called once per frame
	void Update ()
	{ 
        if (enabled)
        {
            if (!player1)
            {
                player1 = GameObject.FindGameObjectWithTag("Player1");
                if (player1)
                {
                    playerOneInfo = player1.GetComponent<PlayerInformation>();
                    playerOneHealth = player1.GetComponent<Health>();
                }
            }
            if (!player2)
            {
                player2 = GameObject.FindGameObjectWithTag("Player2");
                if (player2)
                {
                    playerTwoInfo = player2.GetComponent<PlayerInformation>();
                    playerTwoHealth = player2.GetComponent<Health>();
                }
            }



            //sets the ui to display the correct stat level
            if (player1)
            {
                SetStats(playerOneInfo, playerOneHealth, playerOneLevel);
            }
            if (player2)
            {
                SetStats(playerTwoInfo, playerTwoHealth, playerTwoLevel);
            }
        }
	}

    void SetStats(PlayerInformation pI, Health pH, StatLevel[] pL)
    {
        for (int i = 0; i < stats.Length; i++) 
        {
            if (stats.Length > 0) {
                if (stats[i].statName == StatName.RunSpeed)
                    pL[i].level = GetClosestLevel(StatName.RunSpeed, pI.maxMoveSpeed);
                else if (stats[i].statName == StatName.maxHealth)
                    pL[i].level = GetClosestLevel(StatName.maxHealth, pH.maxHealth);
                else if (stats[i].statName == StatName.AttackSpeed)
                    pL[i].level = GetClosestLevel(StatName.AttackSpeed, pI.attackSpeed);
                else if (stats[i].statName == StatName.Range)
                    pL[i].level = GetClosestLevel(StatName.Range, pI.attackDistance);
                else if (stats[i].statName == StatName.Resistance)
                    pL[i].level = GetClosestLevel(StatName.Resistance, pI.resistance);
                else if (stats[i].statName == StatName.Spread)
                    pL[i].level = GetClosestLevel(StatName.Spread, pI.attackMinAngle);
                else if (stats[i].statName == StatName.Damage)
                    pL[i].level = GetClosestLevel(StatName.Damage, pI.damageOutput);
                else if (stats[i].statName == StatName.Knockback)
                    pL[i].level = GetClosestLevel(StatName.Knockback, pI.knockback);
            }
        }
    }
}
