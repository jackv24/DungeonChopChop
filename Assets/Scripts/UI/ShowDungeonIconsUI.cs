using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDungeonIconsUI : MonoBehaviour {

    private RawImage[] children;

	// Use this for initialization
	void Start () {
		children = GetComponentsInChildren<RawImage> ();

        foreach (RawImage child in children)
            child.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		//sets the correct image if the current dungeon has been completed
        if (ItemsManager.Instance.hasGoggles)
            children[3].gameObject.SetActive(true);
        if (ItemsManager.Instance.hasBoots)
            children[2].gameObject.SetActive(true);
        if (ItemsManager.Instance.hasArmourPiece)
            children[1].gameObject.SetActive(true);
        if (ItemsManager.Instance.hasGauntles)
            children[0].gameObject.SetActive(true);
	}
}
