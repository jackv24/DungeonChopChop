using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum StatName
{
	Range,
	RunSpeed,
	Damage,
	maxHealth,
	Resistance,
	Knockback,
};

[Serializable]
public class Stat
{
	public StatName statName;
    [Tooltip("The value that will indicate the stat is at max")]
	public float maxValue;
}

public class StatsManager : MonoBehaviour 
{
	public Stat[] stats;

	private PlayerInformation playerOneInfo;
	private Health playerOneHealth;

    private PlayerInformation playerTwoInfo;
    private Health playerTwoHealth;

	private GameObject player1;
    private GameObject player2;

    public float GetStat(PlayerInformation player, StatName stat)
    {
        if (stat == StatName.Range)
            return player.attackDistance;
        else if (stat == StatName.Damage)
            return player.damageOutput;
        else if (stat == StatName.RunSpeed)
            return player.maxMoveSpeed;
        else if (stat == StatName.Knockback)
            return player.knockback;
        else if (stat == StatName.maxHealth)
            return player.playerMove.playerHealth.maxHealth;
        else if (stat == StatName.Resistance)
            return player.resistance;
        else
            return 0;
    }

    public float GetMaxStat(StatName stat)
    {
        foreach (Stat s in stats)
        {
            if (s.statName == stat)
            {
                return s.maxValue;
            }
        }

        return 0;
    }
}

