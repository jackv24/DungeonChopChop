using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int startSceneIndex = 1;

    private int currentSceneIndex = -1;

    private Dictionary<string, float> globalMultipliers = new Dictionary<string, float>();
    private Dictionary<string, bool> globalBools = new Dictionary<string, bool>();

    public List<PlayerInformation> players = new List<PlayerInformation>();

    public int playerCount = 0;

    [Header("Global Enemy Values")]
    public float enemyMoveMultiplier;
    public float enemyHealthMultiplier;
    public float enemyStrengthMultiplier;

    [HideInInspector]
    public bool enteredDungeon = false;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);

        if (SceneManager.GetActiveScene().name == "Game")
        {
            StartCoroutine(SetupGame(startSceneIndex));
        }

        if (LevelGenerator.Instance)
            LevelGenerator.Instance.OnTileEnter += CheckTileType;

        //make sure we are in the game scene
        SceneManager.sceneLoaded += SceneChange; 

        StartingItem startingItem = GameObject.FindObjectOfType<StartingItem>();

        if (startingItem)
            startingItem.AddStartingItems();
    }

    void CheckTileType()
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
                    enteredDungeon = true;
                }
            }
        }
    }

    public void Reset()
    {
        players.Clear();
        globalMultipliers.Clear();
        globalBools.Clear();
    }

    void SceneChange(Scene scene, LoadSceneMode mode) 
    { 
        if (enabled)
        {
            if (scene.buildIndex == 1)
                ChangeScene(scene.buildIndex);
        }
    } 

    IEnumerator SetupGame(int index)
    {
        yield return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        currentSceneIndex = index;
        // After first scene is loaded, do game setup
    }

    public void ChangeScene(int newSceneIndex)
    {
        StartCoroutine(ChangeSceneSequence(newSceneIndex));
    }

    IEnumerator ChangeSceneSequence(int index)
    {
        yield return SceneManager.UnloadSceneAsync(currentSceneIndex);

        //stuff inbetween scenes

        AsyncOperation async = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        
        while(!async.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));

        currentSceneIndex = index;
    }

    public void SetGlobalMultiplier(string key, float value)
    {
        globalMultipliers[key] = value;
    }

    public void SetGlobalBool(string key, bool value)
    {
        globalBools[key] = value;
    }

    public float GetGlobalMultiplier(string key)
    {
        if (globalMultipliers.ContainsKey(key))
            return globalMultipliers[key];
        else
            return 1.0f;
    }

    public bool GetGlobalBool(string key)
    {
        if (globalBools.ContainsKey(key))
            return globalBools[key];
        else
            return false;
    }
}
