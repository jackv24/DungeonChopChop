using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
	public Dropdown resolutionDropdown;
	public Dropdown qualityDropdown;

	void Start()
	{
		if(resolutionDropdown)
		{
			//Get list of available resolutions
			List<Resolution> resolutions = new List<Resolution>(Screen.resolutions);

			//Add resolutions as a list of strings
			List<string> resStrings = new List<string>();
			for (int i = 0; i < resolutions.Count; i++)
				resStrings.Add(resolutions[i].ToString());

			//Clear current dropdown
			resolutionDropdown.ClearOptions();

			//Add dropdown options strings and select current resolution
			resolutionDropdown.AddOptions(resStrings);
			resolutionDropdown.value = resolutions.IndexOf(Screen.currentResolution);
		}

		if (qualityDropdown)
		{
			//Get quality level names
			List<string> qualityLevels = new List<string>(QualitySettings.names);

			//Clear quality dropdown
			qualityDropdown.ClearOptions();

			//Add names to dropdown and select current quaity level
			qualityDropdown.AddOptions(qualityLevels);
			qualityDropdown.value = QualitySettings.GetQualityLevel();
		}
	}
}
