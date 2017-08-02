using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour {

	[Header("Items")]
	public int Keys;
	public int Coins;
	[Header("Item Text")]
	public Text keyText;
	public Text coinText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		keyText.text = "" + Keys;
		coinText.text = "" + Coins;
	}
}
