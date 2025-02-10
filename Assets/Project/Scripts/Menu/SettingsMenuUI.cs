using System.Collections;
using CoreTeamGamesSDK.SettingsService;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SettingsMenuUI : MonoBehaviour
{
    #region Variables
    [Header("Video Settings")]
    [SerializeField] private Toggle _fogToggle;
    [SerializeField] private Toggle _shadowsToggle;
    [SerializeField] private Toggle _fulscreenToggle;
    [SerializeField] private Toggle _postProcessToggle;

    [SerializeField] private Slider _drawDistanceSlider;

    [SerializeField] private TMP_Dropdown _resolutionsDropdown;

    [Header("Audio Settings")]
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _uiVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    [Header("Language Settings")]
    [SerializeField] private Button _languageButtonPrefab;

    private SettingsValueWithName[] _values;
    private List<Resolution> _resolutions;
    #endregion

    #region Code
    public void Start()
    {
        Initialize();
        SetupValues(SettingsManager.SettingsValues, false);

    }

    private void Initialize()
    {
        _values = SettingsManager.SettingsValues;
        _resolutionsDropdown.onValueChanged.AddListener(OnResolutionSelect);
        _fulscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        _fogToggle.onValueChanged.AddListener(OnFogChanged);
        _postProcessToggle.onValueChanged.AddListener(OnPostProcessChanged);
        _shadowsToggle.onValueChanged.AddListener(OnShadowsChanged);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
        _drawDistanceSlider.onValueChanged.AddListener(OnDrawDistanceChanged);
    }

    private void OnDrawDistanceChanged(float drawDistance)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "drawdistance").Value = (int)drawDistance*10;
        _values.Single(t => t.Name.ToLower() == "drawdistance").Value = (int)drawDistance*10;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    private void OnUIVolumeChanged(float volume)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "uivolume").Value = volume;
        _values.Single(t => t.Name.ToLower() == "uivolume").Value = volume;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    private void OnMusicVolumeChanged(float volume)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "musicvolume").Value = volume;
        _values.Single(t => t.Name.ToLower() == "musicvolume").Value = volume;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    private void OnSFXVolumeChanged(float volume)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "sfxvolume").Value = volume;
        _values.Single(t => t.Name.ToLower() == "sfxvolume").Value = volume;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    private void OnShadowsChanged(bool isOn)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "shadows").Value = (object)isOn;
        _values.Single(t => t.Name.ToLower() == "shadows").Value = (object)isOn;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    private void OnPostProcessChanged(bool isOn)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "postprocess").Value = (object)isOn;
        _values.Single(t => t.Name.ToLower() == "postprocess").Value = (object)isOn;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    private void OnFogChanged(bool isOn)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "fog").Value = (object)isOn;
        _values.Single(t => t.Name.ToLower() == "fog").Value = (object)isOn;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    public void SetupValues(SettingsValueWithName[] values, bool isDefault)
    {
        foreach (var value in values)
        {
            switch (value.Name.ToLower())
            {
                case "resolution":
                    _resolutions = new List<Resolution>();

                    Resolution loadedResolution = (Resolution)value.Value;

                    int resolutionIndex = 0;
                    bool AddCustomResolution = true;

                    for (int x = 0; x < Screen.resolutions.Length; x++)
                    {
                        _resolutions.Add(Screen.resolutions[x]);
                        if (Screen.resolutions[x].width == loadedResolution.width && Screen.resolutions[x].height == loadedResolution.height && !isDefault)
                        {
                            AddCustomResolution = false;
                            resolutionIndex = x;
                        }
                        else if (isDefault && Screen.resolutions[x].width == Screen.currentResolution.width && Screen.resolutions[x].height == Screen.currentResolution.height)
                        {
                            AddCustomResolution = false;
                            resolutionIndex = x;
                        }
                    }

                    if(AddCustomResolution)
                    {
                        _resolutions.Add(loadedResolution);
                        resolutionIndex = _resolutions.Count;
                    }

                    _resolutionsDropdown.ClearOptions();

                    _resolutionsDropdown.AddOptions(_resolutions.Select((t) => $"{t.width}X{t.height}").ToList());

                    _resolutionsDropdown.SetValueWithoutNotify(resolutionIndex);
                    break;

                case "fullscreen":
                    _fulscreenToggle.SetIsOnWithoutNotify((bool)value.Value);
                    break;

                case "PostProcess":
                    _postProcessToggle.SetIsOnWithoutNotify((bool)value.Value);
                    break;

                case "fog":
                    _fogToggle.SetIsOnWithoutNotify((bool)value.Value);
                    break;

                case "drawdistance":
                    _drawDistanceSlider.SetValueWithoutNotify((int)value.Value/10);
                    break;

                case "shadows":
                    _shadowsToggle.SetIsOnWithoutNotify((bool)value.Value);
                    break;


                default:

                    break;
            }

        }
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "fullscreen").Value = isFullscreen;
        _values.Single(t => t.Name.ToLower() == "fullscreen").Value = isFullscreen;
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }

    public void OnResolutionSelect(int index)
    {
        if (_resolutions.Count == 0)
        {
            Debug.LogError("The resolutions list does not cotain any item!");
            return;
        }
        if (index > _resolutions.Count)
        {
            Debug.LogWarning($"The resolution index ({index}) is bigger than resolutions list length ({_resolutions.Count})");
            return;
        }

        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "resolution").Value = _resolutions[index];
        _values.Single(t => t.Name.ToLower() == "resolution").Value = _resolutions[index];
        SettingsManager.OnSettingsUpdatesEvent?.Invoke(SettingsManager.SettingsValues);
    }
    
    public void SaveSettings()
    {

        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "drawdistance").Value = (int)_drawDistanceSlider.value * 10;
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "uivolume").Value = _uiVolumeSlider.value;
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "musicvolume").Value = _musicVolumeSlider.value;
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "sfxvolume").Value = _sfxVolumeSlider.value;
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "shadows").Value = (object)_shadowsToggle.isOn;
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "postprocess").Value = (object)_postProcessToggle.isOn;
        SettingsManager.SettingsValues.Single(t => t.Name.ToLower() == "fog").Value = (object)_fogToggle.isOn;

        SettingsManager.SaveSettings();
    }
    #endregion
}