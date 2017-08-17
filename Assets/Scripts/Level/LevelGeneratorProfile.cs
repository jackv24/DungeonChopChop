using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorProfile : ScriptableObject
{
	public LevelTile startTile;

	[Space()]
	public int maxTrailLength = 5;
    public int minTileAmount = 5;

	[Space()]
	[Tooltip("The amount of time the level generator will try to generate a level with a minimum tile amount before giving up.")]
	public int maxAttempts = 20;

	[HideInInspector]
	public bool succeeded = true;

    [System.Serializable]
    public class GeneratorTile
    {
        public LevelTile tile;
        public float probability = 1.0f;

        public GeneratorTile()
        {
            tile = null;
            probability = 1.0f;
        }
    }
	[Space()]
	public List<GeneratorTile> tilePool = new List<GeneratorTile>();

	[Header("Item Spawning")]
	[Range(0, 1f)]
	public float chestSpawnProbability = 0.1f;

	public virtual void Generate(LevelGenerator levelGenerator)
	{
		//Default generate function does nothing
		Debug.LogWarning("Base generate function called!");
	}
}
