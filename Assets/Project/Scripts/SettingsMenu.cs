using CoreTeamGamesSDK.SettingsService;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SettingsMenu : MonoBehaviour
{
    private List<Resolution> _resolutions = new List<Resolution>();

    private void Start()
    {
        foreach (var screenResolution in Screen.resolutions)
        {
            _resolutions.Add(screenResolution);
        }

        SettingsManager.OnSettingsUpdatesEvent += Apply;
        SettingsManager.OnSettingsLoadedEvent += Apply;


    }

    private void OnDestroy()
    {
        SettingsManager.OnSettingsUpdatesEvent -= Apply;
        SettingsManager.OnSettingsLoadedEvent -= Apply;
    }

    public void Apply(SettingsValueWithName[] values, bool isDefault)
    {
        foreach (var value in values)
        {
            switch (value.Name.ToLower())
            {
                case "resolution":
                    ChangeResolution((Resolution)value.Value,isDefault);
                    break;

                case "fullscreen":
                    EnableFullscreen((bool)value.Value);
                    break;

                case "postprocess":
                    EnablePostProcess((bool)value.Value);
                    break;

                case "fog":
                    EnableFog((bool)value.Value);
                    break;

                case "drawdistance":
                    ChangeDrawDistance((int)value.Value);
                    break;

                case "shadows":
                    EnableShadows((bool)value.Value);
                    break;


                default:

                    break;
            }

        }
    }

    public void Apply(SettingsValueWithName[] values)
    {
        Apply(values, false);
    }

    private void EnableShadows(bool boolValue)
    {
        Light[] lights = FindObjectsOfType<Light>();

        if (lights.Length != 0)
        {
            foreach (Light light in lights)
            {
                light.shadows = boolValue? LightShadows.Hard: LightShadows.None;
            }
        }
    }

    private void ChangeDrawDistance(float value)
    {
        Camera[] cameras = FindObjectsOfType<Camera>();

        if (cameras.Length != 0)
        {
            foreach (Camera camera in cameras)
            {
                camera.farClipPlane = value;
            }
        }
    }

    private void EnableFog(bool enabled)
    {
        RenderSettings.fog = enabled;
    }

    private void ChangeResolution(Resolution resolution, bool isDefault)
    {
        Resolution curentResolution = resolution;

        if (isDefault)
        {
            curentResolution = Screen.currentResolution;
        }

        Screen.SetResolution(curentResolution.width, curentResolution.height, Screen.fullScreen);
    }

    private void EnableFullscreen(bool boolValue)
    {
        Screen.fullScreen = boolValue;
        Screen.fullScreenMode = boolValue ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void EnablePostProcess(bool enabled)
    {
        Camera[] cameras = FindObjectsOfType<Camera>();

        if (cameras.Length != 0)
        {
            foreach (Camera camera in cameras)
            {
                if (camera.TryGetComponent<PostProcessLayer>(out PostProcessLayer layer))
                {
                    layer.enabled = enabled;
                }
            }
        }
    }
}
