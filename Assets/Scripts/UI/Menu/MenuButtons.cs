using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
	public int SceneIndex = 1;
	public float sceneLoadDelay = 0.5f;

	private bool clicked = false;

	public Image fadeImage;

    public void MainMenu()
    {
        ObjectPooler.PurgePools();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

	public void ClickSinglePlayer()
    {
        //gets canvas
        Canvas canv = GameObject.FindObjectOfType<Canvas>();
        canv.enabled = false;

        if (!clicked)
        {
            clicked = true;
            StartCoroutine(ChangeSceneDelay(SceneIndex));
        }
    }

	public void ClickCoOp()
    {

        //gets canvas
        Canvas canv = GameObject.FindObjectOfType<Canvas>();
        canv.enabled = false;

        if (!clicked)
        {
            clicked = true;
            StartCoroutine(ChangeSceneDelay(SceneIndex));
        }
    }

	IEnumerator ChangeSceneDelay(int index)
	{
		if (fadeImage)
		{
			float elapsed = 0;

			Color color = fadeImage.color;
			color.a = 0;

			while (elapsed < sceneLoadDelay)
			{
				color.a = elapsed / sceneLoadDelay;
				fadeImage.color = color;

				yield return new WaitForEndOfFrame();
				elapsed += Time.deltaTime;
			}

			color.a = 1;
			fadeImage.color = color;

			yield return new WaitForEndOfFrame();
		}
		else
			yield return new WaitForSeconds(sceneLoadDelay);

        SceneManager.LoadScene("Overworld");
	}

    public void Replay()
    {
        ObjectPooler.PurgePools();

        PlayerPrefs.SetInt("SkipMenu", 1);
        PlayerPrefs.SetInt("PlayerCount", GameManager.Instance.players.Count);
        SceneManager.LoadScene("Game");
        Debug.Log(PlayerPrefs.GetInt("SkipMenu"));
    }
}
