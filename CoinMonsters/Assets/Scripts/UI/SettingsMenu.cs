using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    public Slider audioSlider;
    public Toggle fullScreenToggle;
    public TMP_Dropdown qualityDropdown;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    List<Resolution> allowedResolutions = new List<Resolution>();

    private void Awake()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate <= 60)
            {

                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                allowedResolutions.Add(resolutions[i]);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResolutionIndex = allowedResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);

        fullScreenToggle.isOn = Screen.fullScreen;

        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
        resolutionDropdown.value = savedResolutionIndex;
        Screen.SetResolution(allowedResolutions[savedResolutionIndex].width, allowedResolutions[savedResolutionIndex].height, Screen.fullScreen);
        resolutionDropdown.RefreshShownValue();

        float savedVolume = PlayerPrefs.GetFloat("volume", 0f);
        audioSlider.value = savedVolume;
        audioMixer.SetFloat("volume", savedVolume);

        int qualityLevel = PlayerPrefs.GetInt("UnityGraphicsQuality", 2);
        qualityDropdown.value = qualityLevel;
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("UnityGraphicsQuality", qualityIndex);
    }

    public void SetFullscreen(bool toggle)
    {
        Screen.fullScreen = toggle;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = allowedResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

}
