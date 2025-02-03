using CoreTeamGamesSDK.SettingsService;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SettingsApplyer : MonoBehaviour
{
    SettingsManager _manager;
    [SerializeField] Vector2Int[] _resolutions;

    private void Awake()
    {
        _manager = FindObjectOfType<SettingsManager>();

        if (_manager == null)
        {
            Debug.LogError($"Can{"'"}t find SettingsManager!");
            return;
        }

        _manager.OnValuesUpdateEvent += OnValuesUpdateOrLoad;
        _manager.OnValuesLoadedEvent += OnValuesUpdateOrLoad;

    }

    private void OnDestroy()
    {
        _manager.OnValuesUpdateEvent -= OnValuesUpdateOrLoad;
        _manager.OnValuesLoadedEvent -= OnValuesUpdateOrLoad;
    }

    public void OnValuesUpdateOrLoad(SettingsValue[] values)
    {
        foreach (var value in values)
        {
            switch (value.ValueName.ToLower())
            {
                case "resolution":
                    ChangeResolution(value.intValue);
                    break;

                case "fullscreen":
                    EnableFullscreen(value.boolValue);
                    break;

                case "enablepostprocess":
                    EnablePostProcess(value.boolValue);
                    break;

                case "fog":
                    EnableFog(value.boolValue);
                    break;

                case "drawdistance":
                    ChangeDrawDistance(value.floatValue);
                    break;

                case "shadows":
                    EnableShadows(value.boolValue);
                    break;


                default:

                    break;
            }
           
        }
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

    private void ChangeResolution(int intValue)
    {
        if (intValue <= _resolutions.Length)
            Screen.SetResolution(_resolutions[intValue].x, _resolutions[intValue].y, Screen.fullScreen);
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
