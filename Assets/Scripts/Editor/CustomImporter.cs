using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomImporter : AssetPostprocessor
{
	void OnPreprocessModel()
	{
		ModelImporter modelImporter = (ModelImporter)assetImporter;

		//Prevent newly imported models from generating their own materials - these should be purposefully assigned
		modelImporter.importMaterials = false;

		//Most models will not use animation, and don't need animators by default
		//modelImporter.animationType = ModelImporterAnimationType.None;
	}
}
