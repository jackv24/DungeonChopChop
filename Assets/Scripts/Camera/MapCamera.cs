using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCamera : MonoBehaviour
{
	public static MapCamera Instance;

	public Canvas canvas;
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
				//Transform position from world to map viewport
				Vector2 viewPos = cam.WorldToViewportPoint(icon.targetTransform.position);
				//Map to actual UI image
				Vector2 localPos = new Vector2(viewPos.x * mapRect.sizeDelta.x, viewPos.y * mapRect.sizeDelta.y);
				Vector3 worldPos = mapRect.TransformPoint(localPos);

				//Set icon position
				icon.rectTransform.position = new Vector3(worldPos.x - mapRect.sizeDelta.x, worldPos.y, 1f);

				//Make sure icon does not go off screen
				LimitToRadius(icon.rectTransform, rawMapRect, mapRadius);
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

	public void RegisterIcon(Sprite sprite, Transform target, Color color, bool setLastSibling = false)
	{
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
