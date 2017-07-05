using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    public GameObject grassDecalPrefab;
    public GameObject desertDecalPrefab;
    public GameObject fireDecalPrefab;

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
        }

        if (newGraphic)
        {
            GameObject obj = Instantiate(newGraphic, transform.parent);
            obj.transform.localPosition = transform.localPosition;
            obj.transform.localRotation = transform.localRotation;

            DestroyImmediate(gameObject);
        }
    }
}
