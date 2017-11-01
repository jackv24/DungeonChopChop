using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGlobalItems : MonoBehaviour {

    public Text keyText;
	public Image key;
    public Text coinText;
	public Image coin;

	// Use this for initialization
	void Start () {
		ItemsManager.Instance.OnCoinChange += TriggerCoin;
		ItemsManager.Instance.OnKeyChange += TriggerKey;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        keyText.text = "" + ItemsManager.Instance.Keys;
        coinText.text = "" + ItemsManager.Instance.Coins;
	}

	IEnumerator BoolWait(string boolName, Animator anim)
	{
		anim.SetBool (boolName, true);
		yield return new WaitForSeconds (.1f);
		anim.SetBool (boolName, false);
	}

	void TriggerKey()
	{
		if (key) {
			if (key.GetComponent<Animator> ())
				StartCoroutine(BoolWait ("Animate", key.GetComponent<Animator> ()));
		}
	}

	void TriggerCoin()
	{
		if (coin) {
			if (coin.GetComponent<Animator> ())
				StartCoroutine(BoolWait ("Animate", coin.GetComponent<Animator> ()));
		}
	}
}
