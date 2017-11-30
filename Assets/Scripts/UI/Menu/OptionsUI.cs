using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class OptionsUI : MonoBehaviour
{
	public GameObject firstSelected;
	private GameObject lastSelected;

	public Dropdown resolutionDropdown;
	public Toggle fullscreenToggle;
	public Dropdown qualityDropdown;

    public Toggle postFXToggle;

	private List<Resolution> resolutions;

	public Slider masterVolume;
    public Slider musicVolume;
    public AudioMixer audioMixer;

    void Start()
	{
		if (Pause.Instance)
			Pause.Instance.OnUnpause += Close;

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

        if (audioMixer)
        {
            if (masterVolume)
                masterVolume.onValueChanged.AddListener((float value) => { audioMixer.SetFloat("MasterVolume", Helper.LinearToDecibel(value)); });

            if (musicVolume)
				musicVolume.onValueChanged.AddListener((float value) => { audioMixer.SetFloat("MusicVolume", Helper.LinearToDecibel(value)); });
        }

        if(postFXToggle)
        {
            postFXToggle.onValueChanged.AddListener((bool value) =>
            {
                var postFX = FindObjectOfType<UnityEngine.PostProcessing.PostProcessingBehaviour>();

                if (postFX)
                    postFX.enabled = value;
            });
        }
    }

	void Close()
	{
		transform.parent.gameObject.SetActive(false);
	}

	void OnEnable()
	{
		lastSelected = EventSystem.current.currentSelectedGameObject;

		if(firstSelected)
			StartCoroutine(SwitchSelected(firstSelected));

		if (masterVolume && PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolume.value = PlayerPrefs.GetFloat("MasterVolume");
        }

        if (musicVolume && PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        if(postFXToggle && PlayerPrefs.HasKey("PostFX"))
        {
            postFXToggle.isOn = PlayerPrefs.GetInt("PostFX") > 0 ? true : false;
        }
    }

	void OnDisable()
	{
		if (lastSelected && EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(lastSelected);
			EventSystem.current.firstSelectedGameObject = lastSelected;
		}

		if (Pause.Instance)
			Pause.Instance.OnUnpause -= Close;

		if(masterVolume)
            PlayerPrefs.SetFloat("MasterVolume", masterVolume.value);

		if (musicVolume)
            PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);

        if (postFXToggle)
            PlayerPrefs.SetInt("PostFX", postFXToggle.isOn ? 1 : 0);
    }

	IEnumerator SwitchSelected(GameObject newObject)
	{
		EventSystem.current.SetSelectedGameObject(null);

		yield return new WaitForEndOfFrame();

		EventSystem.current.SetSelectedGameObject(newObject);
		EventSystem.current.firstSelectedGameObject = newObject;
	}

	void UpdateResolution()
	{
		Resolution res = resolutions[resolutionDropdown.value];

		Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
	}
}
