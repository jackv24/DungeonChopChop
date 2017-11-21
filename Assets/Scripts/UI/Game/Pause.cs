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

    [Header("Panels")]
    public GameObject pauseMenu;
    public GameObject player1Stats;
    public GameObject player2Stats;
    public GameObject MainMenuOptions;
    public GameObject statistics;
    public GameObject statsScreen;
    public GameObject warpToTownOptions;
    public GameObject optionsPanel;

    [Header("Selected GameObjects")]
    public GameObject firstSelected;
    public GameObject MainMenuOptionsFirstSelected;
    public GameObject warpToTownFirstSelected;

    [Header("Buttons")]
    public GameObject warpToTownButton;

    [Space()]

    public bool paused = false;
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

    void FixedUpdate()
    {
        if (LevelGenerator.Instance)
        {
            if (LevelGenerator.Instance.currentTile)
            {
                if (LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.Dungeon1 ||
                LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.Dungeon2 ||
                LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.Dungeon3 ||
                LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.Dungeon4 ||
                LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.BossDungeon)
                {
                    warpToTownButton.SetActive(true);
                }
                else
                    warpToTownButton.SetActive(false);
            }
        }
    }

	void ShowStatsScreen()
	{
        if (!DeathScreen.deathScreenShown)
        {
            statsDisplayed = true;

            SlowTime(1, 0);

            if (statsScreen)
                statsScreen.SetActive(true);

            paused = true;
        }
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
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
            EventSystem.current.firstSelectedGameObject = firstSelected;
        }
    }

    public void UnPauseGame(bool slowDown = true)
    {
        pauseMenu.SetActive(false);
        paused = false;

        if(slowDown)
		    SlowTime(0, 1);

		if (OnUnpause != null)
			OnUnpause();

        HidePanels();
    }

    void HidePanels()
    {
        statistics.SetActive(false);
        MainMenuOptions.SetActive(false);
        warpToTownOptions.SetActive(false);
        optionsPanel.SetActive(false);
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

    public void OpenWarpToTownOption()
    {
        warpToTownOptions.SetActive(true);

        EventSystem.current.SetSelectedGameObject(warpToTownFirstSelected.gameObject);
        EventSystem.current.firstSelectedGameObject = warpToTownFirstSelected.gameObject;
    }

    public void WarpToTown()
    {
        ItemsManager.Instance.Coins = 0;

        LevelGeneratorProfile profile = LevelVars.Instance.levelData.overworldProfile;
        int seed = LevelVars.Instance.levelData.overworldSeed;

        if (LevelGenerator.Instance && profile)
        {
            LevelGenerator.Instance.RegenerateWithProfile(profile, seed);
        }

        UnPauseGame();
    }

    public void ExitOption()
    {
        MainMenuOptions.SetActive(true);

        EventSystem.current.SetSelectedGameObject(MainMenuOptionsFirstSelected.gameObject);
        EventSystem.current.firstSelectedGameObject = MainMenuOptionsFirstSelected.gameObject;
    }

    public void ExitNo()
    {
        MainMenuOptions.SetActive(false);
        warpToTownOptions.SetActive(false);

		EventSystem.current.SetSelectedGameObject (firstSelected);
        EventSystem.current.firstSelectedGameObject = firstSelected;
    }
}
