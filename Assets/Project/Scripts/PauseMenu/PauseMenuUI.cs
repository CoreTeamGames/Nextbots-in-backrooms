﻿using UnityEngine;

[RequireComponent(typeof(PauseMenu))]
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private UIWindow _uIWindow;
    private CursorLockMode _cursorLockModeBeforePause;
    private bool _cursorVisibilityBeforePause;

    void Awake()
    {
        PauseMenu.OnGamePauseEvent += OnPause;

        _cursorLockModeBeforePause = Cursor.lockState;
        _cursorVisibilityBeforePause = Cursor.visible;
    }

    private void OnDestroy()
    {
        PauseMenu.OnGamePauseEvent -= OnPause;
    }

    // Update is called once per frame
    void OnPause(bool isPaused)
    {
        if (_uIWindow == null)
            return;

        if (isPaused)
        {
            _cursorLockModeBeforePause = Cursor.lockState;
            _cursorVisibilityBeforePause = Cursor.visible;

            _uIWindow.ShowWindow();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            _uIWindow.HideWindow();


            Cursor.lockState = _cursorLockModeBeforePause;
            Cursor.visible = _cursorVisibilityBeforePause;
        }
    }
}