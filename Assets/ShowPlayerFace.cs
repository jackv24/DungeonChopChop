using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlayerFace : MonoBehaviour {

    public string playerTag;
    public RenderTexture renderTexture;

    private GameObject player;
    private Camera playerCamera;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player)
            playerCamera = player.GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        playerCamera.targetTexture = renderTexture;
	}
}
