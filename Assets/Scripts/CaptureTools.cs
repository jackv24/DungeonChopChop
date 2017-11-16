using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTools : MonoBehaviour
{
	void Update()
	{
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
		{
			CaptureScreenshot();
		}
	}

	void CaptureScreenshot()
	{
		string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
		string name = path + System.IO.Path.DirectorySeparatorChar + "ChopChop_" + System.DateTime.Now.ToString().Replace(" ", "_").Replace("/", "-").Replace(":", "-") + ".png";

		ScreenCapture.CaptureScreenshot(name);

		Debug.Log("Saved screenshot to: " + name);
	}
}
