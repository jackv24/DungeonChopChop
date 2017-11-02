using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDungeonIconsUI : MonoBehaviour {

	public Sprite[] dungeonIcons;

	private Image[] children;

	// Use this for initialization
	void Start () {
		children = GetComponentsInChildren<Image> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		//sets the correct image if the current dungeon has been completed
		if (ItemsManager.Instance.hasGoggles)
			children [0].sprite = dungeonIcons [0];
		if (ItemsManager.Instance.hasBoots)
			children [1].sprite = dungeonIcons [1];
		if (ItemsManager.Instance.hasArmourPiece)
			children [2].sprite = dungeonIcons [2];
		if (ItemsManager.Instance.hasGauntles)
			children [3].sprite = dungeonIcons [3];
	}
}
