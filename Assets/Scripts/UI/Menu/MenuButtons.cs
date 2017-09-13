using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {

	public int SceneIndex = 2;
	public float sceneLoadDelay = 0.5f;

	private bool clicked = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ClickSinglePlayer()
	{
		if (!clicked)
		{
			clicked = true;
			StartCoroutine(ChangeSceneDelay(SceneIndex));
		}
	}

	public void ClickCoOp()
	{
		if (!clicked)
		{
			clicked = true;
			StartCoroutine(ChangeSceneDelay(SceneIndex));
		}
	}

	IEnumerator ChangeSceneDelay(int index)
	{
		yield return new WaitForSeconds(sceneLoadDelay);

		GameManager.Instance.ChangeScene(index);
	}

	public void ClickOptions()
	{

	}

}
