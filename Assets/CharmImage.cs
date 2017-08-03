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
		//loops through for the amount of charms possible
		for (int i = 0; i < playerInfo.charmAmount; i++)
		{
			Image img = null;

			if (i < charmImages.Count)
				img = charmImages [i];
			else
			{
				//creates the ui image
				GameObject obj = Instantiate (imagePrefab, transform);
				//sets the scale of the ui image to 1
				obj.transform.localScale = Vector3.one;

				img = obj.GetComponent<Image> ();
				//adds the image to the list
				charmImages.Add (img);
			}
			if (i < playerInfo.currentCharms.Count)
			{
				if (img)
					//sets the sprite image to the current charms icon
					img.sprite = playerInfo.currentCharms [i].itemIcon;
			}
			else
			{
				img.sprite = null;
			}
		}
	}
}
