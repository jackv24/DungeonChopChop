using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
	[System.Serializable]
	public class MaterialPair
	{
		public Material insideMaterial;
		public Material outsideMaterial;
	}

	public MaterialPair grassMap;
	public MaterialPair forestMap;
	public MaterialPair desertMap;
	public MaterialPair iceMap;
	public MaterialPair fireMap;

	public LevelTile.Biomes biome;

	private MeshRenderer rend;

	void Awake()
	{
		rend = GetComponentInChildren<MeshRenderer>();
	}

	void Start()
	{
		if (rend)
			rend.enabled = false;
	}

	public void SetInside()
	{
		if (rend)
		{
			rend.enabled = true;
			rend.sharedMaterial = GetMaterialPair().insideMaterial;
		}
	}

	public void SetOutside()
	{
		if (rend)
		{
			rend.enabled = true;
			rend.sharedMaterial = GetMaterialPair().outsideMaterial;
		}
	}

	MaterialPair GetMaterialPair()
	{
		switch(biome)
		{
			case LevelTile.Biomes.Grass:
				return grassMap;
			case LevelTile.Biomes.Desert:
				return desertMap;
			case LevelTile.Biomes.Fire:
				return fireMap;
			case LevelTile.Biomes.Ice:
				return iceMap;
			case LevelTile.Biomes.Forest:
				return forestMap;
		}

		return null;
	}
}
