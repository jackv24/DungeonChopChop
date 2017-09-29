﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCamera : MonoBehaviour
{
	public static MapCamera Instance;

	public Canvas canvas;
	private CanvasScaler canvasScaler;

	public RectTransform mapRect;
	public RectTransform rawMapRect;

	[Header("Icons")]
	public float iconScale = 1.0f;
	public float mapRadius = 1.0f;

	class Icon
	{
		public RectTransform rectTransform;
		public Transform targetTransform;

		public bool setLastSibling = false;
	}

	private List<Icon> icons = new List<Icon>();

	private float height;
	private CameraFollow cameraFollow;
	private Camera cam;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		height = transform.position.y;
		cameraFollow = FindObjectOfType<CameraFollow>();
		cam = GetComponent<Camera>();

		if (LevelGenerator.Instance)
			LevelGenerator.Instance.OnGenerationStart += ClearIcons;

		if (canvas)
			canvasScaler = canvas.GetComponent<CanvasScaler>();
	}

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	void LateUpdate()
	{
		if (cameraFollow)
		{
			transform.position = cameraFollow.targetPos + Vector3.up * height;
		}

		if(cam && mapRect)
		{
			//Update all icons
			foreach (Icon icon in icons)
			{
				if (icon.targetTransform)
				{
					//Transform position from world to map viewport
					Vector2 viewPos = cam.WorldToViewportPoint(icon.targetTransform.position);
					//Map to actual UI image
					Vector2 localPos = new Vector2(viewPos.x * mapRect.sizeDelta.x, viewPos.y * mapRect.sizeDelta.y);
					Vector3 worldPos = mapRect.TransformPoint(localPos);

					float ratio = Screen.height / canvasScaler.referenceResolution.y;

					//Set icon position
					icon.rectTransform.position = new Vector3(worldPos.x - mapRect.sizeDelta.x * ratio, worldPos.y, 1f);

					//Make sure icon does not go off screen
					LimitToRadius(icon.rectTransform, rawMapRect, mapRadius * ratio);
				}
			}
		}
	}

	public void ClearIcons()
	{
		foreach(Icon icon in icons)
		{
			Destroy(icon.rectTransform.gameObject);
		}
		icons.Clear();
	}

	public void RegisterPlayers()
	{
		//Re-register players after clearing
		PlayerInformation[] players = FindObjectsOfType<PlayerInformation>();
		foreach (PlayerInformation player in players)
		{
			MapTracker tracker = player.GetComponent<MapTracker>();

			if (tracker)
				tracker.Register();
		}
	}

	/// <summary>
	/// Displays an icon on the map.
	/// </summary>
	/// <param name="sprite">The sprite to use for the icon.</param>
	/// <param name="target">World transform that the icon represents.</param>
	/// <param name="color">Tint color.</param>
	/// <param name="setLastSibling">Sets this icon as the last sibling in the heirarchy, drawing above other icons.</param>
	/// <returns>Returns the assigned ID of the new icon.</returns>
	public void RegisterIcon(Sprite sprite, Transform target, Color color, bool setLastSibling = false)
	{
		//Only add tracker if this transform isn't already tracked
		foreach(Icon i in icons)
		{
			if (i.targetTransform == target)
				return;
		}

		//Create new icon and set values
		Icon icon = new Icon();
		icon.targetTransform = target;
		icon.setLastSibling = setLastSibling;

		//Make new gameobject in heirarchy for this icon
		GameObject obj = new GameObject(target.gameObject.name + "_Icon");

		//Add gameobject to canvas with canvas renderer
		icon.rectTransform = obj.AddComponent<RectTransform>();
		obj.AddComponent<CanvasRenderer>();

		//Add image component and set sprite, size, color
		Image image = obj.AddComponent<Image>();
		image.sprite = sprite;
		image.SetNativeSize();
		image.color = color;

		//Make sure icon is child of canvas
		if (canvas)
			obj.transform.SetParent(canvas.transform, false);

		obj.transform.localScale *= iconScale;

		//Add to list
		icons.Add(icon);

		//Reorder in heirarchy if required
		SetIconOrder();
	}

	public void RemoveIcon(Transform t)
	{
		int index = -1;

		//Find icon with ID
		foreach(Icon icon in icons)
		{
			if (icon.targetTransform == t)
				index = icons.IndexOf(icon);
		}

		//If icon match was found
		if (index >= 0)
		{
			//Delete from UI
			Destroy(icons[index].rectTransform.gameObject);

			//Remove icon from list
			icons.RemoveAt(index);
		}
	}

	void SetIconOrder()
	{
		foreach (Icon icon in icons)
		{
			//Reorder in heirarchy if required
			if (icon.setLastSibling)
				icon.rectTransform.SetAsLastSibling();
		}
	}

	void LimitToRadius(RectTransform icon, RectTransform parent, float radius)
	{
		Vector2 offset = (Vector2)icon.position - (Vector2)parent.position;

		//If icon is fruther away from the centre than the radius
		if (offset.magnitude > radius)
		{
			//Normalize offset the get direction, then multiply by radius to get new offset
			Vector3 direction = offset.normalized;
			Vector3 newOffset = direction * radius;

			//Set position as new offset
			icon.position = newOffset + parent.position;
		}
	}
}
