using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Helper
{
	[MenuItem("Jack's Helper Functions/Reduce Materials")]
	private static void CleanupMaterials()
	{
		if(EditorUtility.DisplayDialog("Reduce materials?", "All objects in scene will be reduced to 1 material each. Please make sure this is what you intended, and remember to apply any prefabs afterwards.", "Clean Up!", "Cancel"))
		{
			MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

			//Loop through all mesh renderers in scene
			foreach(MeshRenderer rend in renderers)
			{
				//If mesh renderer has more than 1 material it should be reduced to 1
				if(rend.sharedMaterials.Length > 1)
				{
					Material mat = rend.sharedMaterials[0];

					Material[] materials = new Material[1];
					materials[0] = mat;

					rend.sharedMaterials = materials;
				}
			}
		}
	}
}
