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

	[Space()]
	[Tooltip("The amount of time the level generator will try to generate a level with a minimum tile amount before giving up.")]
	public int maxAttempts = 20;

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
