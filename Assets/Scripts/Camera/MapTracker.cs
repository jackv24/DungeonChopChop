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

	public bool registerOnce = false;
	private bool registered = false;

	private LevelTile parentTile;

	void Start()
	{
		if (sprite)
			Setup();
	}

	public void Setup()
	{
		if (MapCamera.Instance)
		{
			if (showOnTileEnter)
			{
				parentTile = GetComponentInParent<LevelTile>();

				if (parentTile)
					parentTile.OnTileEnter += Register;
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

		if (MapCamera.Instance)
			Remove(false);
	}

	public void Register()
	{
		if (registerOnce && registered)
			return;

		MapCamera.Instance.RegisterIcon(sprite, transform, color, setLastSibling);

		registered = true;
	}

	public void Remove()
	{
		Remove(true);
	}

	public void Remove(bool unsubcribe)
	{
		MapCamera.Instance.RemoveIcon(transform);

		if(unsubcribe)
		{
			if(showOnTileEnter)
			{
				if (parentTile)
					parentTile.OnTileEnter -= Register;
			}
			else
			{
				if (LevelGenerator.Instance)
					LevelGenerator.Instance.OnGenerationFinished -= Register;
			}
		}
	}
}
