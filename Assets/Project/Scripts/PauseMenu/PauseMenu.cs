using CoreTeamGamesSDK.Input.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool canPause = true;

    public static bool IsPaused { get; private set; }

    private float _timeScaleBeforePause;

    private float _TimeScale { get => Time.timeScale; set => Time.timeScale = value; }

    public delegate void OnGamePause(bool isPaused);
    public static OnGamePause OnGamePauseEvent;

    private void OnEnable()
    {
        canPause = true;

        InputManager.Instance.SubscribeToInput("Pause", Pause);

        _timeScaleBeforePause = _TimeScale;
    }

    private void OnDisable()
    {
        InputManager.Instance.UnsubscribeFromInput("Pause", Pause);
    }

    private void Pause(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!canPause)
            return;

        Pause(!IsPaused);
    }

    public void Pause()
    {
        if (!canPause)
            return;

        Pause(!IsPaused);
    }

    public void Pause(bool isPaused)
    {
        if (!canPause)
            return;

        if (!IsPaused)
        {
            IsPaused = true;
            _timeScaleBeforePause = _TimeScale;
            _TimeScale = 0;
        }
        else
        {
            IsPaused = false;
            _TimeScale = _timeScaleBeforePause;
        }

        OnGamePauseEvent?.Invoke(IsPaused);
    }
}
