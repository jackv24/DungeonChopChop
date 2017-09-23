using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
	public Dropdown resolutionDropdown;
	public Toggle fullscreenToggle;
	public Dropdown qualityDropdown;

	private List<Resolution> resolutions;

	void Start()
	{
		if(resolutionDropdown)
		{
			//Get list of available resolutions
			resolutions = new List<Resolution>(Screen.resolutions);

			//Add resolutions as a list of strings
			List<string> resStrings = new List<string>();
			for (int i = 0; i < resolutions.Count; i++)
				resStrings.Add(resolutions[i].ToString());

			//Clear current dropdown
			resolutionDropdown.ClearOptions();

			//Add dropdown options strings and select current resolution
			resolutionDropdown.AddOptions(resStrings);
			resolutionDropdown.value = resolutions.IndexOf(Screen.currentResolution);

			resolutionDropdown.onValueChanged.AddListener(delegate { UpdateResolution(); });

			if (fullscreenToggle)
			{
				fullscreenToggle.isOn = Screen.fullScreen;

				fullscreenToggle.onValueChanged.AddListener(delegate { UpdateResolution(); });
			}
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

			qualityDropdown.onValueChanged.AddListener((int index) => { QualitySettings.SetQualityLevel(index, true); });
		}
	}

	void UpdateResolution()
	{
		Resolution res = resolutions[resolutionDropdown.value];

		Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
	}
}
