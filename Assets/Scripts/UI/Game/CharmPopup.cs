using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharmPopup : MonoBehaviour
{
	public AnimationClip closeAnim;

	[Space()]
	public Text titleText;
	public Text descriptionText;

	[Space()]
	public MeshRenderer[] iconRenderers;

	[Space()]
	public Vector3 offset = new Vector3(0, 2, 1);
	private Transform followTransform;

	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
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

	public void Init(Charm charm, Transform player)
	{
		titleText.text = charm.displayName;
		descriptionText.text = charm.itemInfo;

		if (iconRenderers.Length > 0)
		{
			Material mat = iconRenderers[0].material;

			if (charm.itemIcon)
				mat.mainTexture = charm.itemIcon.texture;
			else
				mat.mainTexture = null;

			foreach (MeshRenderer iconRenderer in iconRenderers)
			{
				iconRenderer.sharedMaterial = mat;
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
