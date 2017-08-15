using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    public GameObject grassDecalPrefab;
    public GameObject desertDecalPrefab;
    public GameObject fireDecalPrefab;
	public GameObject iceDecalPrefab;
	public GameObject forestDecalPrefab;
	public GameObject dungeonDecalPrefab;

	private GameObject oldGraphic = null;

    public void Replace(LevelTile.Biomes biome)
    {
        GameObject newGraphic = null;

        switch (biome)
        {
            case LevelTile.Biomes.Grass:
                newGraphic = grassDecalPrefab;
                break;
            case LevelTile.Biomes.Desert:
                newGraphic = desertDecalPrefab;
                break;
            case LevelTile.Biomes.Fire:
                newGraphic = fireDecalPrefab;
                break;
			case LevelTile.Biomes.Ice:
				newGraphic = iceDecalPrefab;
				break;
			case LevelTile.Biomes.Forest:
				newGraphic = forestDecalPrefab;
				break;
			case LevelTile.Biomes.Dungeon:
				newGraphic = dungeonDecalPrefab;
				break;
		}

        if (newGraphic)
        {
            GameObject obj = Instantiate(newGraphic, transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

			if(oldGraphic)
				DestroyImmediate(oldGraphic);

			oldGraphic = obj;
		}
    }
}
