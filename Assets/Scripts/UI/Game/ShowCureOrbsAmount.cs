using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCureOrbsAmount : MonoBehaviour {

    public string playerTag;

    private Text orbAmountText;
    private GameObject player;
    private PlayerInformation playerInfo;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
        {
            playerInfo = player.GetComponent<PlayerInformation>();
            playerInfo.cureOrbChange += SetText;
        }
        orbAmountText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetText()
    {
        orbAmountText.text = playerInfo.currentCureOrbs + "/" + playerInfo.maxCureOrbs;

    }
}
