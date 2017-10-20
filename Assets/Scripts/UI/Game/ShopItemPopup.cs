using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPopup : MonoBehaviour
{
	public AnimationClip closeAnim;

	[Space()]
	public Text titleText;
	public Text descriptionText;

    public RawImage renderImage;
    private Camera cam;

	[Space()]
    public Transform meshParent;
    public string meshLayer = "UI_Pickup";

    [Space()]
	public Vector3 offset = new Vector3(0, 2, 1);
	private Transform followTransform;

	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	void Start()
	{
        cam = GetComponentInChildren<Camera>();

		//Set up new render texture
        if(cam && renderImage)
		{
            Vector2 size = Vector2.one;

            LayoutElement layout = renderImage.GetComponent<LayoutElement>();
            size.x = layout.minWidth;
            size.y = layout.minHeight;

            //Make render texture match size of image in UI
            RenderTexture texture = new RenderTexture((int)size.x, (int)size.y, 16, RenderTextureFormat.ARGB32);

            renderImage.texture = texture;
            cam.targetTexture = texture;
        }
	}

	void OnEnable()
	{
		if (closeAnim && anim)
			StartCoroutine(CloseAnimation());
	}

	void Update()
	{
        if (followTransform)
		    transform.position = followTransform.position + offset;
	}

	public void Init(InventoryItem item, Transform player)
	{
		titleText.text = item.displayName;
		descriptionText.text = item.itemInfo;

        //Display mesh
        if (meshParent && item.itemPrefab)
        {
            meshParent.gameObject.DestroyChildren();

            GameObject itemGraphic = Instantiate(item.itemPrefab, meshParent);
            itemGraphic.transform.localPosition = item.popupOffset.position;
            itemGraphic.transform.localEulerAngles = item.popupOffset.rotation;
			
			Vector3 scale = itemGraphic.transform.localScale;
            scale.x *= item.popupOffset.scale.x;
            scale.y *= item.popupOffset.scale.y;
            scale.z *= item.popupOffset.scale.z;
            itemGraphic.transform.localScale = scale;

            itemGraphic.SetLayerWithChildren(LayerMask.NameToLayer(meshLayer));

            //Only need to display this item, don't need any behaviours
            Component[] components = itemGraphic.GetComponentsInChildren<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (!(components[i] is MeshRenderer
                || components[i] is MeshFilter
                || components[i] is Transform
                || components[i] is ParticleSystem
                || components[i] is ParticleSystemRenderer))
                    DestroyImmediate(components[i], false);
            }
        }

        followTransform = player;
	}

	IEnumerator CloseAnimation()
	{
		// Wait until anim is finished
		yield return new WaitForSeconds(closeAnim.length);

		// Disable for pooling
		gameObject.SetActive(false);
	}
}
