using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelGen", menuName = "Data/Level Generator Profile")]
public class LevelGeneratorProfile : ScriptableObject
{
	public LevelTile startTile;

	[Space()]
	public int maxTrailLength = 5;
    public int minTileAmount = 5;
    public int maxTileAmount = 20;

    [System.Serializable]
    public class GeneratorTile
    {
        public LevelTile tile;
        public float probability;

        public GeneratorTile()
        {
            tile = null;
            probability = 1.0f;
        }
    }
	[Space()]
	public List<GeneratorTile> tilePool = new List<GeneratorTile>();
}
