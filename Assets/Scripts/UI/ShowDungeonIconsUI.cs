using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDungeonIconsUI : MonoBehaviour {

	private Image[] children;

	// Use this for initialization
	void Start () {
		children = GetComponentsInChildren<Image> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		//sets the correct image if the current dungeon has been completed
        if (ItemsManager.Instance.hasGoggles)
            children[0].enabled = true;
        if (ItemsManager.Instance.hasBoots)
            children[1].enabled = true;
        if (ItemsManager.Instance.hasArmourPiece)
            children[2].enabled = true;
        if (ItemsManager.Instance.hasGauntles)
            children[3].enabled = true;
	}
}
