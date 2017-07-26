using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class CoOpButton : MonoBehaviour {

	Button button;

	// Use this for initialization
	void Start () {
		button = GetComponent<Button> ();
	}

	// Update is called once per frame
	void Update () {
		//grey if controller is connected
		if (InControl.InputManager.Devices.Count > 0) 
		{
			button.interactable = true;
		} 
		else 
		{
			button.interactable = false;
		}
	}
}
