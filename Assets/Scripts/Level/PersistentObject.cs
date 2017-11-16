﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
	[Tooltip("If left blank, GameObject name will be used")]
    public string identifier = "";
    public bool onStart = false;
    public bool useChild = false;

    private bool boolSet = false;

    private static Dictionary<string, bool> dictionary = new Dictionary<string, bool>();

    public static void Reset()
    {
        if(dictionary != null)
            dictionary.Clear();
    }

	void Start()
	{
        if (onStart)
            Setup();
    }

	public void Setup()
	{
		if(useChild && transform.childCount > 0 && GetPersistentBool())
		{
            GameObject obj = transform.GetChild(0).gameObject;

            EventSender sender = obj.AddComponent<EventSender>();
            sender.disableOnLevelClear = true;

            sender.OnDisableEvent += delegate { SetPersistentBool(false); };
        }
		else
			gameObject.SetActive(GetPersistentBool());
	}

	bool GetPersistentBool()
	{
        bool value = true;

        if (dictionary.ContainsKey(identifier))
        {
            value = dictionary[identifier];
            boolSet = true;
        }

        return value;
    }

	public void SetPersistentBool(bool value)
	{
		if(dictionary.ContainsKey(identifier))
            Debug.LogWarning("Persistent Bool Dictionary already contains key \"" + identifier + "\"");
		else
            dictionary.Add(identifier, value);
	}
}
