using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharmImage : MonoBehaviour {

	public int id = 0;
	public GameObject imagePrefab;

	private List<Image> charmImages = new List<Image> ();

	public void UpdateCharms(PlayerInformation playerInfo)
	{
		for (int i = 0; i < playerInfo.charmAmount; i++)
		{
			Image img = null;

			if (i < charmImages.Count)
				img = charmImages [i];
			else
			{
				GameObject obj = Instantiate (imagePrefab, transform);
				obj.transform.localScale = Vector3.one;

				img = obj.GetComponent<Image> ();
				charmImages.Add (img);
			}

			if (i < playerInfo.currentCharms.Count)
			{
				if (img)
					img.sprite = playerInfo.currentCharms [i].itemIcon;
			}
			else
			{
				img.sprite = null;
			}
		}
	}
}
