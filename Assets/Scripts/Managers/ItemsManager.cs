using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour {

    public static ItemsManager Instance;

	[Header("Items")]
	public int Keys;
	public int DungeonKeys;
	public int Coins;

    [Space()]
    public bool hasGoggles = false;
    public bool hasBoots = false;
    public bool hasArmourPiece = false;
    public bool hasGauntles = false;

    [Header("Global Variables")]
    public int itemDropMultiplier;

    void Awake()
    {
        Instance = this;
    }

    public void Reset()
    {
        Keys = 0;
        DungeonKeys = 0;
        Coins = 0;

        hasGoggles = false;
        hasBoots = false;
        hasArmourPiece = false;
        hasGauntles = false;
    }
}
