using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopwatchUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _text;
    private Stopwatch _stopwatch;

    private void Awake()
    {
        _stopwatch = new Stopwatch();
        PauseMenu.OnGamePauseEvent += OnGamePause;
    }

    private void Update()
    {
        if (!_stopwatch.IsRunning)
                return;

        _text.text = $"{_stopwatch.Minutes}:{_stopwatch.Seconds}:{_stopwatch.Miliseconds}";
    }

    public void StartStopwatch()
    {
        _stopwatch.Start();
    }

    public void StopStopwatch()
    {
        _stopwatch.Stop();
    }

    public void OnGamePause(bool isPaused)
    {
        if (isPaused)
            _stopwatch.Pause();
        else
            _stopwatch.Start();
    }
}
