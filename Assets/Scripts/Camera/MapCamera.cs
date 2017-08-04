using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCamera : MonoBehaviour
{
	public static MapCamera Instance;

	public RectTransform mapRect;

	[Header("Transforms")]
	public Transform town;

	[Header("Icons")]
	public Sprite townIcon;
	private RectTransform townIconTransform;
	[Space()]
	public float iconScale = 1.0f;

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

		InitialiseIcons();
	}

	void LateUpdate()
	{
		if (cameraFollow)
		{
			transform.position = cameraFollow.targetPos + Vector3.up * height;
		}

		if(cam && mapRect)
		{
			if(town && townIconTransform)
			{
				Vector2 viewPos = cam.WorldToViewportPoint(town.position);
				Vector2 localPos = new Vector2(viewPos.x * mapRect.sizeDelta.x, viewPos.y * mapRect.sizeDelta.y);
				Vector3 worldPos = mapRect.TransformPoint(localPos);

				townIconTransform.position = new Vector3(worldPos.x - mapRect.sizeDelta.x, worldPos.y, 1f);
			}
		}
	}

	void InitialiseIcons()
	{
		GameObject obj = new GameObject("TownIcon");

		townIconTransform = obj.AddComponent<RectTransform>();
		obj.AddComponent<CanvasRenderer>();

		Image townIconImage = obj.AddComponent<Image>();
		townIconImage.sprite = townIcon;
		townIconImage.SetNativeSize();

		if (mapRect)
			obj.transform.SetParent(mapRect.transform, false);

		obj.transform.localScale *= iconScale;
	}
}
