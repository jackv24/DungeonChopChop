using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeLighting : MonoBehaviour
{
	public Light sunLight;

	[System.Serializable]
	public class LightProfile
	{
		public Color sunColor = Color.white;
		public float sunIntensity = 1;

		[Space()]
		public Material skyboxMaterial;
		public float ambientIntensity = 1.0f;
	}

	public LightProfile grassProfile;
	public LightProfile forestProfile;
	public LightProfile fireProfile;
	public LightProfile iceProfile;
	public LightProfile desertProfile;
	public LightProfile dungeonProfile;

	void Start()
	{
		//if a sun light has not been assigned, find one
		if(!sunLight)
		{
			GameObject obj = GameObject.FindWithTag("SunLight");

			if (obj)
				sunLight = obj.GetComponent<Light>();
		}

		//Update lighting when entering tiles
		if(LevelGenerator.Instance)
			LevelGenerator.Instance.OnTileEnter += UpdateLighting;
	}

	void OnDestroy()
	{
		if (LevelGenerator.Instance)
			LevelGenerator.Instance.OnTileEnter -= UpdateLighting;
	}

	LightProfile GetProfile(LevelTile.Biomes biome)
	{
		//Return correct biome profile
		switch(biome)
		{
			case LevelTile.Biomes.Grass:
				return grassProfile;
			case LevelTile.Biomes.Forest:
				return forestProfile;
			case LevelTile.Biomes.Fire:
				return fireProfile;
			case LevelTile.Biomes.Ice:
				return iceProfile;
			case LevelTile.Biomes.Desert:
				return desertProfile;
			case LevelTile.Biomes.Dungeon:
				return dungeonProfile;
		}

		return null;
	}

	void UpdateLighting()
	{
		//Get profile for current biome
		LightProfile profile = GetProfile(LevelGenerator.Instance.currentTile.Biome);

		if (profile != null)
		{
			if (sunLight)
			{
				sunLight.color = profile.sunColor;
				sunLight.intensity = profile.sunIntensity;

				//Disable the sun if intensity is below zero
				if (sunLight.intensity <= 0)
					sunLight.gameObject.SetActive(false);
				else
					sunLight.gameObject.SetActive(true);
			}
			else
				Debug.LogWarning("No sun found to update!");

			RenderSettings.skybox = profile.skyboxMaterial;
			RenderSettings.ambientIntensity = profile.ambientIntensity;
		}
		else
			Debug.LogWarning("No profile for current biome!");
	}
}
