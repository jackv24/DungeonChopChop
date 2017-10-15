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

    [HideInInspector]
    //0 = don't skip, 1 = do skip
    public int skipMenu;
    public int playerCount = 0;

    void Awake()
    {
        Instance = this;
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
