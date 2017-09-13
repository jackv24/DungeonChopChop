using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

    public GameObject pauseMenu;

    bool paused = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) || InControl.InputManager.ActiveDevice.CommandWasPressed)
        {
            if (paused)
            {
                UnPauseGame();
            }
            else
            {
                PauseGame();
            }
        }
	}

    void PauseGame()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        paused = false;
    }

    public void MainMenu()
    {
        ObjectPooler.PurgePools();

		if (LevelGenerator.Instance)
			LevelGenerator.Instance.Clear();

		UnPauseGame();

        SceneManager.LoadScene("Game");
    }
}
