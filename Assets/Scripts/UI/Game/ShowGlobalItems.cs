using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGlobalItems : MonoBehaviour {

    public Text keyText;
    public Text coinText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        keyText.text = "" + ItemsManager.Instance.Keys;
        coinText.text = "" + ItemsManager.Instance.Coins;
	}
}
