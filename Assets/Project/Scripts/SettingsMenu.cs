using CoreTeamGamesSDK.SettingsService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SettingsMenu : MonoBehaviour
{
    private void Start()
    {
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
                    ChangeResolution((Resolution)value.Value, isDefault);
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
                case "sensivity":
                    ChangeSensivity((float)value.Value);
                    break;

                default:

                    break;
            }

        }
    }

    private void ChangeSensivity(float value)
    {
        CameraController camera = FindObjectOfType<CameraController>();

        if (camera == null)
            return;

        camera.SetSensivity(value);
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
                light.shadows = boolValue ? LightShadows.Hard : LightShadows.None;
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
        Resolution currentResolution = new Resolution() { width = Display.main.renderingWidth, height = Display.main.renderingHeight };

        if (!isDefault || ( resolution.width > 0 && resolution.height > 0))
            currentResolution = resolution;

        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
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
