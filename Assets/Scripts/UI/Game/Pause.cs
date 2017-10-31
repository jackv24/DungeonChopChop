﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
	public static Pause Instance;

	public delegate void NormalEvent();
	public event NormalEvent OnUnpause;

	public GameObject firstSelected;

    public GameObject pauseMenu;
    public GameObject player1Stats;
    public GameObject player2Stats;
    public GameObject YesOrNoPanel;

    public GameObject statistics;


    bool paused = false;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
        PlayerInformation[] players = FindObjectsOfType<PlayerInformation>();
        if (players.Length < 2)
        {
            player2Stats.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
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

		if (firstSelected)
			EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        paused = false;

		if (OnUnpause != null)
			OnUnpause();

        statistics.SetActive(false);
        YesOrNoPanel.SetActive(false);
    }

    public void MainMenu()
    {
        ObjectPooler.PurgePools();

		if (LevelGenerator.Instance)
			LevelGenerator.Instance.Clear();

		UnPauseGame();

        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        InputManager im = GameObject.FindObjectOfType<InputManager>();
        PlayerManager pm = GameObject.FindObjectOfType<PlayerManager>();

        if (gm)
            Destroy(gm.gameObject);
        if (im)
            Destroy(im.gameObject);
        if (pm)
            Destroy(pm.gameObject);

        SceneManager.LoadScene("MainMenu");
    }

    public void ExitOption()
    {
        YesOrNoPanel.SetActive(true);
    }

    public void ExitNo()
    {
        YesOrNoPanel.SetActive(false);
    }
}
