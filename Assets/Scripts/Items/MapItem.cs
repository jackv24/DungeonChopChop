﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Item", menuName = "Data/Items/Map Item")]
public class MapItem : BaseItem
{
	[Space()]
	public bool revealGrass = false;
	public bool revealForest = false;
	public bool revealDesert = false;
	public bool revealIce = false;
	public bool revealFire = false;

	[Space()]
	public bool revealChests = false;
	public bool revealShops = false;
	public bool revealHearts = false;

	[Space()]
	public bool revealDungeonForest = false;
	public bool revealDungeonDesert = false;
	public bool revealDungeonIce = false;
	public bool revealDungeonFire = false;

	public override void Pickup(PlayerInformation playerInfo)
	{
		if(LevelGenerator.Instance)
		{
			foreach(LevelTile tile in LevelGenerator.Instance.generatedTiles)
			{
				bool shouldReveal = false;
				bool hasDungeonPin = false;

				switch(tile.Biome)
				{
					case LevelTile.Biomes.Grass:
						shouldReveal = revealGrass;
						break;
					case LevelTile.Biomes.Forest:
						shouldReveal = revealForest;
						hasDungeonPin = revealDungeonForest;
						break;
					case LevelTile.Biomes.Desert:
						shouldReveal = revealDesert;
						hasDungeonPin = revealDungeonDesert;
						break;
					case LevelTile.Biomes.Fire:
						shouldReveal = revealFire;
						hasDungeonPin = revealDungeonFire;
						break;
					case LevelTile.Biomes.Ice:
						shouldReveal = revealIce;
						hasDungeonPin = revealDungeonIce;
						break;
				}

				if(shouldReveal && tile != LevelGenerator.Instance.currentTile)
					tile.ShowTile(false, false);

				if (hasDungeonPin)
				{
					DungeonEntrance dungeon = tile.GetComponentInChildren<DungeonEntrance>();

					if(dungeon)
					{
						tile.Reveal();
					}
				}
			}
		}
	}
}
