using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enum represents all in-game sounds
/// </summary>
public enum GameSound
{
    CorrectAnswer,
    WrongAnswer,
    Countdown,
    GameEnd,
    GameStart,
    EasterEgg
}

/// <summary>
/// Interface for a game sound manager
/// </summary>
public interface ISoundManager
{
    /// <summary>
    /// Start playing main theme
    /// </summary>
    void PlayMainTheme();

    /// <summary>
    /// Play sound once
    /// </summary>
    void PlaySound(GameSound sound);
}
