using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
	[System.Serializable]
	public class StatsUI
	{
		[HideInInspector]
        public PlayerInformation player;
        [HideInInspector]
        public GameObject cameraPrefab;
		private static float lastYPos = 0;

        public RawImage armourImage;
		public RawImage bootImage;
		public RawImage swordImage;

		public Camera RenderImage(RawImage image, InventoryItem item)
		{
			if(!item.itemPrefab)
                return null;

            GameObject cameraObj = Instantiate(cameraPrefab);
            Camera cam = cameraObj.GetComponent<Camera>();

            CanvasScaler scaler = image.GetComponentInParent<CanvasScaler>();
            float scale = 1.0f;

			if(scaler)
                scale = Screen.height / scaler.referenceResolution.y;

            RenderTexture texture = new RenderTexture((int)(image.rectTransform.sizeDelta.x * scale), (int)(image.rectTransform.sizeDelta.y * scale), 0, RenderTextureFormat.ARGB32);
            cam.targetTexture = texture;
            image.texture = texture;
            image.color = Color.white;

            GameObject obj = Instantiate(item.itemPrefab, cameraObj.transform);
            obj.transform.localPosition = item.popupOffset.position;
            obj.transform.localEulerAngles = item.popupOffset.rotation;

            float offset = cam.orthographicSize * 2 + lastYPos;
            cameraObj.transform.position += Vector3.up * offset;
            lastYPos = offset;

            Vector3 s = obj.transform.localScale;
            s.x *= item.popupOffset.scale.x;
            s.y *= item.popupOffset.scale.y;
            s.z *= item.popupOffset.scale.z;
            obj.transform.localScale = s;

            obj.SetLayerWithChildren(LayerMask.NameToLayer("UI_Pickup"));

            //Only need to display this item, don't need any behaviours
            Component[] components = obj.GetComponentsInChildren<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (!(components[i] is MeshRenderer
                || components[i] is MeshFilter
                || components[i] is Transform
                || components[i] is ParticleSystem
                || components[i] is ParticleSystemRenderer))
                    DestroyImmediate(components[i], false);
            }

            return cam;
        }

		public List<Camera> RenderAll()
		{
			if(!player)
                return null;

            InventoryItem armourItem = null;
			InventoryItem bootItem = null;
			InventoryItem swordItem = null;

			foreach(InventoryItem item in player.currentItems)
			{
				if(armourItem == null && item.armourType == ArmourType.ChestPiece)
                    armourItem = item;
				else if (bootItem == null && item.armourType == ArmourType.Boots)
                    bootItem = item;
				else
                    swordItem = item;
            }

            List<Camera> camList = new List<Camera>();

            if(armourImage && armourItem)
                camList.Add(RenderImage(armourImage, armourItem));

			if (bootImage && bootItem)
                camList.Add(RenderImage(bootImage, bootItem));

			if (swordImage && swordItem)
                camList.Add(RenderImage(swordImage, swordItem));

            return camList;
        }
    }

    public GameObject cameraPrefab;

    [Space()]
    public StatsUI player1Stats;
    public StatsUI player2Stats;

    private List<Camera> cameraList = new List<Camera>();

    void OnEnable()
	{
        GameObject player1 = GameObject.FindWithTag("Player1");
		GameObject player2 = GameObject.FindWithTag("Player2");

		if(player1)
		{
            PlayerInformation info = player1.GetComponent<PlayerInformation>();

			if(info)
			{
                player1Stats.player = info;
                player1Stats.cameraPrefab = cameraPrefab;
                cameraList.AddRange(player1Stats.RenderAll());
            }
        }

		if (player2)
        {
            PlayerInformation info = player2.GetComponent<PlayerInformation>();

            if (info)
            {
                player2Stats.player = info;
				player2Stats.cameraPrefab = cameraPrefab;
                cameraList.AddRange(player2Stats.RenderAll());
            }
        }
    }

	void OnDisable()
	{
        foreach (Camera cam in cameraList)
        {
			if(cam)
            	Destroy(cam.gameObject);
        }

        cameraList.Clear();
    }
}
