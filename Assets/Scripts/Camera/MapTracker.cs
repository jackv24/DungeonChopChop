using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTracker : MonoBehaviour
{
	public Sprite sprite;
	public Color color = Color.white;

	[Space()]
	public bool setLastSibling = false;

	[Space()]
	public bool showOnTileEnter = false;

	private int id = -1;

	void Start()
	{
		if (sprite && MapCamera.Instance)
		{
			if (showOnTileEnter)
			{
				LevelTile tile = GetComponentInParent<LevelTile>();

				if (tile)
					tile.OnTileEnter += Register;
			}
			else
			{
				if (LevelGenerator.Instance)
					LevelGenerator.Instance.OnGenerationFinished += Register;
				else
					Register();
			}
		}
	}

	void OnDestroy()
	{
		if (LevelGenerator.Instance)
			LevelGenerator.Instance.OnGenerationFinished -= Register;
	}

	public void Register()
	{
		if(id < 0)
			id = MapCamera.Instance.RegisterIcon(sprite, transform, color, setLastSibling);
	}

	public void Remove()
	{
		if(id >= 0)
			MapCamera.Instance.RemoveIcon(id);
	}
}
