using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
	public int SceneIndex = 1;
	public float sceneLoadDelay = 0.25f;

	private bool clicked = false;

	public Image fadeImage;

    public void MainMenu()
    {
        ObjectPooler.PurgePools();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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

        yield return new WaitForSeconds(LoadingScreen.Show("Loading", true));

        AsyncOperation async = SceneManager.LoadSceneAsync("Overworld");
	}

    public void Replay()
    {
        ObjectPooler.PurgePools();

        PersistentObject.Reset();

        GameManager gm = GameObject.FindObjectOfType<GameManager>();

        if (gm)
            Destroy(gm.gameObject);

        ItemsManager.Instance.Reset();

        SceneManager.LoadScene("Overworld");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
