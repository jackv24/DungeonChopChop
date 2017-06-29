using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame()
	{
		GameManager.Instance.ChangeScene (2);
	}

}
