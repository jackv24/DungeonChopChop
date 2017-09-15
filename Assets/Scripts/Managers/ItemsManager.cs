using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour {

    public static ItemsManager Instance;

	[Header("Items")]
	public int Keys;
	public int Coins;

    [Space()]
    public bool hasGoggles = false;
    public bool hasBoots = false;

    void Awake()
    {
        Instance = this;
    }
}
