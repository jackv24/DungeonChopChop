using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatShow : MonoBehaviour {

	public StatName statName;
    public string playerTag;

	private StatsManager statsManager;
    private PlayerInformation playerInfo;

    private Image bar;

	// Use this for initialization
	void Start ()
	{
        bar = GetComponent<Image>();

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
            playerInfo = player.GetComponent<PlayerInformation>();

		GameObject stat = GameObject.FindGameObjectWithTag ("StatsManager");

		if (stat) 
		{
			statsManager = stat.GetComponent<StatsManager>();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (statsManager) 
		{
            bar.fillAmount = statsManager.GetStat(playerInfo, statName) / statsManager.GetMaxStat(statName);
		}
	}
}
