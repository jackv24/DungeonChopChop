using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeLighting : MonoBehaviour
{
	public Light sunLight;

	[Space()]
	public LightProfile grassProfile;
	public LightProfile forestProfile;
	public LightProfile fireProfile;
	public LightProfile iceProfile;
	public LightProfile desertProfile;
	[Space()]
	public LightProfile dungeon1Profile;
	public LightProfile dungeon2Profile;
	public LightProfile dungeon3Profile;
	public LightProfile dungeon4Profile;
	public LightProfile bossDungeonProfile;

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
			case LevelTile.Biomes.Dungeon1:
				return dungeon1Profile;
			case LevelTile.Biomes.Dungeon2:
                return dungeon2Profile;
			case LevelTile.Biomes.Dungeon3:
                return dungeon3Profile;
			case LevelTile.Biomes.Dungeon4:
				return dungeon4Profile;
			case LevelTile.Biomes.BossDungeon:
				return bossDungeonProfile;
		}

		return null;
	}

	public void UpdateLighting()
	{
        UpdateLighting(null);
    }

	public void UpdateLighting(LightProfile profile)
	{
		//Get profile for current biome
		if(profile == null)
			profile = GetProfile(LevelGenerator.Instance.currentTile.Biome);

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
            RenderSettings.ambientLight = profile.ambientColour;
            RenderSettings.ambientIntensity = profile.ambientIntensity;

			//Set light intensity for each player
			PlayerInformation[] players = FindObjectsOfType<PlayerInformation>();

			foreach(PlayerInformation player in players)
			{
				Light light = player.GetComponentInChildren<Light>(true);

				if (light)
				{
					light.intensity = profile.playerLightIntensity;

					if (light.intensity <= 0)
						light.gameObject.SetActive(false);
					else
						light.gameObject.SetActive(true);
				}
			}
		}
		else
			Debug.LogWarning("No profile for current biome!");
	}
}
