using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class OptionsMenu_Behaviour : MonoBehaviour
{
	public AudioMixer audioMixer;
	public TMPro.TMP_Dropdown resolutionDropdown;

	Resolution[] resolutions;

	private void Start()
	{
		resolutions = Screen.resolutions;

		resolutionDropdown.ClearOptions();

		int currentResolutionIndex = 0;
		List<string> resolutionStrings = new List<string>();
		for(int i = 0; i < resolutions.Length; i++)
		{
			resolutionStrings.Add(resolutions[i].ToString());

			if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
			{
				currentResolutionIndex = i;
			}
		}

		resolutionDropdown.AddOptions(resolutionStrings);
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();
	}

	public void SetResolution(int resolutionIndex)
	{
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	public void SetVolume(float volume)
	{
		audioMixer.SetFloat("MasterVolume", volume);
	}

	public void SetQuality(int qualityIndex)
	{
		QualitySettings.SetQualityLevel(qualityIndex);
	}

	public void SetFullscreen(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}
}
