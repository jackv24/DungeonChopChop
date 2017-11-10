using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveManager
{
	[System.Serializable]
	public class SaveData
	{
        public List<int> failedSeeds = new List<int>();
		public List<int> succeededSeeds = new List<int>();
    }

    private static SaveData data = null;

    private const string dataPath = "/data.sav";

	public static void Init()
	{
		if(data == null)
            Load();
    }

    public static void Load()
	{
        string path = Application.streamingAssetsPath + dataPath;

        if(File.Exists(path))
		{
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(json);
        }
		else
		{
            data = new SaveData();
        }
	}

	public static void Save()
	{
		string path = Application.streamingAssetsPath + dataPath;

        string json = JsonUtility.ToJson(data, true);
        
		System.IO.FileInfo file = new System.IO.FileInfo(path);
        file.Directory.Create(); // If the directory already exists, this method does nothing.
        System.IO.File.WriteAllText(file.FullName, json);
    }

	public static bool CheckSeed(int seed)
	{
        Init();

        bool success = true;

        for (int i = 0; i < data.failedSeeds.Count; i++)
		{
			if(seed == data.failedSeeds[i])
			{
                success = false;
                Debug.Log("Seed: " + seed + " is a known failed seed, skipping...");
                break;
            }
		}

        return success;
    }

	public static void RegisterFailedSeed(int seed)
	{
        Init();

		if(!data.failedSeeds.Contains(seed))
        	data.failedSeeds.Add(seed);

        Debug.Log("Seed: " + seed + " failed, registering to failed seeds...");

        Save();
    }

	public static void RegisterSucceededSeed(int seed)
    {
        Init();

        if (!data.succeededSeeds.Contains(seed))
            data.succeededSeeds.Add(seed);

        Debug.Log("Seed: " + seed + " succeeded!, registering...");

        Save();
    }

	public static int GetSeed()
	{
        Init();

        int seed = 0;

        if(data.succeededSeeds.Count > 0)
            seed = data.succeededSeeds[Random.Range(0, data.succeededSeeds.Count)];

        return seed;
    }
}
