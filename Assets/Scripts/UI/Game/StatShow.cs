using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatShow : MonoBehaviour {

	public StatName statName;
	public RawImage[] statImages;
    public string playerTag;

	private StatsManager statsManager;
    private PlayerInformation playerInfo;

	// Use this for initialization
	void Start ()
	{
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
            playerInfo = player.GetComponent<PlayerInformation>();
        
		foreach (RawImage image in statImages) 
		{
			image.gameObject.SetActive (false);
		}

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
            for (int i = 0; i < statsManager.GetStatLevel (statName, playerInfo); i++) 
			{
				statImages [i].gameObject.SetActive (true);
			}
            for (int i = statsManager.GetStatLevel (statName, playerInfo); i < statImages.Length; i++) 
			{
				statImages [i].gameObject.SetActive (false);
			}
		}
	}
}
