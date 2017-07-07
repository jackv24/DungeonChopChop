using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatShow : MonoBehaviour {

	public StatName statName;
	public RawImage[] statImages;

	private StatsManager statsManager;

	// Use this for initialization
	void Start ()
	{
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
			for (int i = 0; i < statsManager.GetStatLevel (statName); i++) 
			{
				statImages [i].gameObject.SetActive (true);
			}
			for (int i = statsManager.GetStatLevel (statName); i < statImages.Length; i++) 
			{
				statImages [i].gameObject.SetActive (false);
			}
		}
	}
}
