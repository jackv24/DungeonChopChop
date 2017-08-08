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
				Register();
		}
	}

	void Register()
	{
		MapCamera.Instance.RegisterIcon(sprite, transform, color, setLastSibling);
	}
}
