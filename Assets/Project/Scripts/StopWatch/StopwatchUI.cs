using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopwatchUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _text;
    [SerializeField] private float _fadeDuration = 0.2f;
    private Stopwatch _stopwatch;

    private void Awake()
    {
        _stopwatch = new Stopwatch();
        PauseMenu.OnGamePauseEvent += OnGamePause;
        _text.alpha = 0;
    }

    private void Update()
    {
        if (!_stopwatch.IsRunning)
            return;

        _text.text = $"{_stopwatch.Minutes}:{_stopwatch.Seconds}:{_stopwatch.Miliseconds}";
    }

    public void StartStopwatch()
    {
        if (LevelManager.IsGameStarted)
        {
            _stopwatch.Start();
            ShowStopwatch(true);
        }
    }

    public void StopStopwatch()
    {
        if (LevelManager.IsGameStarted)
        {
            _stopwatch.Stop();
            ShowStopwatch(false);
        }
    }

    public void OnGamePause(bool isPaused)
    {
        if (isPaused)
            _stopwatch.Pause();
        else
            _stopwatch.Start();
    }

    public void ShowStopwatch(bool isShow = true)
    {
        _text.DOFade(isShow ? 1 : 0, _fadeDuration).SetUpdate(true);
    }
}