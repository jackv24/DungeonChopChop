using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCureOrbsAmount : MonoBehaviour {

    public string playerTag;

    private GameObject player;
    private PlayerInformation playerInfo;

    private Image cureBar;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag(playerTag);
        cureBar = GetComponent<Image>();
        if (player)
        {
            playerInfo = player.GetComponent<PlayerInformation>();
            //playerInfo.cureOrbChange += CureOrbUpdate;
        }
	}

    void Update()
    {
        cureBar.fillAmount = (float)playerInfo.currentCureAmount / 100;
    }
}
