using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDungeonKeys : MonoBehaviour {

	public Image dungkey;

	// Use this for initialization
	void Start () 
	{
		ItemsManager.Instance.OnDungeonKeyChange += RefreshKeys;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void RefreshKeys()
	{
		Image[] images = GetComponentsInChildren<Image> ();
		foreach (Image img in images) {
			Destroy (img.gameObject);
		}

		for (int i = 0; i < ItemsManager.Instance.DungeonKeys; i++) 
		{
			Image img = (Image)Instantiate (dungkey, transform.position, Quaternion.Euler (0, 0, 0));
			img.transform.parent = transform;
		}
	}
}
