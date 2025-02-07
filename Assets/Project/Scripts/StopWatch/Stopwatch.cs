using System;

public class Stopwatch
{
    private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
    private bool _resetOnStart = false;

    public bool IsRunning => _stopwatch.IsRunning;
    public bool IsPaused { get; private set; }

    public int Miliseconds => GetMiliseconds();
    public int Seconds => GetSeconds();
    public int Minutes => GetMinutes();
    public int Hours => GetHours();

    public void Start()
    {
        if (IsRunning)
            return;

        if (_resetOnStart)
            _stopwatch.Reset();

        IsPaused = false;
        _resetOnStart = false;
        _stopwatch.Start();
    }

    public void Pause()
    {
        if (IsPaused)
            return;

        IsPaused = true;
        _stopwatch.Stop();
    }

    public void Stop()
    {
        IsPaused = false;
        _resetOnStart = true;
        _stopwatch.Stop();
    }

    public int GetMiliseconds(bool getAll = false)
    {
        TimeSpan timeSpan = _stopwatch.Elapsed;
        return getAll ? (int)timeSpan.TotalMilliseconds : (int)timeSpan.Milliseconds;
    }

    public int GetSeconds(bool getAll = false)
    {
        TimeSpan timeSpan = _stopwatch.Elapsed;
        return getAll ? (int)timeSpan.TotalSeconds : (int)timeSpan.Seconds;
    }

    public int GetMinutes(bool getAll = false)
    {
        TimeSpan timeSpan = _stopwatch.Elapsed;
        return getAll ? (int)timeSpan.TotalMinutes : (int)timeSpan.Minutes;
    }

    public int GetHours(bool getAll = false)
    {

        TimeSpan timeSpan = _stopwatch.Elapsed;
        return getAll ? (int)timeSpan.TotalHours : (int)timeSpan.Hours;
    }
}