using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
	public enum SpecialType
	{
		Shop,
		Heart,
		Teacher
	}

	[System.Serializable]
	public class ReplaceBiome
	{
        public GameObject forestPrefab;
		public GameObject icePrefab;
		public GameObject firePrefab;
		public GameObject desertPrefab;
    }

	public ReplaceBiome[] replaceTiles = new ReplaceBiome[System.Enum.GetNames(typeof(SpecialType)).Length];

    private LevelTile tile;

	void Awake()
	{
        tile = GetComponent<LevelTile>();
    }

    public bool Replace(SpecialType specialType)
	{
        if (tile && tile.currentGraphic)
        {
            GameObject replaceTile = null;
            ReplaceBiome replace = replaceTiles[(int)specialType];

			switch(tile.Biome)
			{
				case LevelTile.Biomes.Forest:
                    replaceTile = replace.forestPrefab;
                    break;
				case LevelTile.Biomes.Ice:
                    replaceTile = replace.icePrefab;
                    break;
				case LevelTile.Biomes.Fire:
                    replaceTile = replace.firePrefab;
                    break;
				case LevelTile.Biomes.Desert:
                    replaceTile = replace.desertPrefab;
                    break;
            }

            if (replaceTile)
            {
                GameObject obj = (GameObject)Instantiate(replaceTile, tile.currentGraphic.transform.parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;

                //Remove this tile graphic
                Destroy(tile.currentGraphic);

                tile.currentGraphic = obj;

                return true;
            }
        }

        return false;
    }
}
