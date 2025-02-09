using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static EGameModes CurrentGameMode { get; set; } = EGameModes.Nextbots;
    public static Texture2D CurrentMazeTexture { get; set; }
    public static bool ConsoleEnabled { get; private set; }

}