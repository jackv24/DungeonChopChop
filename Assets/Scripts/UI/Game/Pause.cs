using System.Collections;
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
    public GameObject No;

    public GameObject statistics;
	public GameObject statsScreen;


    bool paused = false;
	bool statsDisplayed = false;

    public float pauseSlowTime = 0.5f;
    private Coroutine pauseRoutine;

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
		if (Input.GetKeyDown (KeyCode.Escape) || InControl.InputManager.ActiveDevice.CommandWasPressed) {
			if (paused)
				UnPauseGame ();
			else
				PauseGame ();
		}

		if (Input.GetKeyDown (KeyCode.Tab) || InControl.InputManager.ActiveDevice.Action4.WasPressed) {
			if (statsDisplayed)
				UnShowStatsScreen ();
			else
				ShowStatsScreen ();
		}
	}

	void ShowStatsScreen()
	{
		statsDisplayed = true;

		SlowTime(1, 0);

		if (statsScreen)
			statsScreen.SetActive (true);

		paused = true;
	}

	void UnShowStatsScreen()
	{
		statsScreen.SetActive (false);

		if (statsScreen)
			statsDisplayed = false;

		if (paused)
			UnPauseGame ();
	}

    void PauseGame()
    {
        paused = true;
        pauseMenu.SetActive(true);

        SlowTime(1, 0);

        if (firstSelected)
			EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void UnPauseGame(bool slowDown = true)
    {
        pauseMenu.SetActive(false);
        paused = false;

        if(slowDown)
		    SlowTime(0, 1);

		if (OnUnpause != null)
			OnUnpause();

        statistics.SetActive(false);
        YesOrNoPanel.SetActive(false);
    }

	void SlowTime(float oldScale, float newScale)
	{
		if(pauseRoutine != null)
            StopCoroutine(pauseRoutine);

        StartCoroutine(SlowTimeOverTime(oldScale, newScale));
    }

	IEnumerator SlowTimeOverTime(float oldScale, float newScale)
	{
        float elapsed = 0;

        Time.timeScale = oldScale;

        while(elapsed < pauseSlowTime)
		{
            Time.timeScale = Mathf.Lerp(oldScale, newScale, elapsed / pauseSlowTime);

            yield return new WaitForEndOfFrame();
            elapsed += Time.unscaledDeltaTime;
        }

		Time.timeScale = newScale;
    }

    public void MainMenu()
    {
        ObjectPooler.PurgePools();

        PersistentObject.Reset();

        if (LevelGenerator.Instance)
			LevelGenerator.Instance.Clear();

		UnPauseGame(false);

        Time.timeScale = 1;

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
        EventSystem.current.SetSelectedGameObject (No.gameObject);
    }

    public void ExitNo()
    {
        YesOrNoPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject (firstSelected);
    }
}
