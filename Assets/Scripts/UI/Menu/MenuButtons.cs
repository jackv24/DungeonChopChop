using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {

	public int SceneIndex = 2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ClickSinglePlayer()
	{
		GameManager.Instance.ChangeScene (SceneIndex);
	}

	public void ClickCoOp()
	{
		GameManager.Instance.ChangeScene (SceneIndex);
	}

	public void ClickOptions()
	{

	}

}
