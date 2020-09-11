using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// All panels available in game menu
/// </summary>
public enum UIMenuPanel
{
    None,
    MainMenu,
    Quiz,
    Help,
    Countdown,
    GameResult,
}

/// <summary>
/// This interface represents game menu UI.
/// </summary>
public interface IUIMenuController
{
    /// <summary>
    /// Called when "Start Game" button pressed in main menu
    /// </summary>
    event Action OnMainMenuStartGamePressed;

    /// <summary>
    /// Called when "Help" button pressed in main menu
    /// </summary>
    event Action OnMainMenuHelpPressed;

    /// <summary>
    /// Called when "Score" button pressed in main menu
    /// </summary>
    event Action OnMainMenuScoreTablePressed;

    /// <summary>
    /// Called when "Submit" button pressed in help board
    /// </summary>
    event Action OnHelpSubmitPressed;

    /// <summary>
    /// Called when "Cancel" button pressed in help board
    /// </summary>
    event Action OnHelpCancelPressed;

    /// <summary>
    /// Called when "Stop Game" button pressed in quiz board
    /// </summary>
    event Action OnQuizStopGamePressed;

    /// <summary>
    /// Called when "Exit" button pressed in main menu
    /// </summary>
    event Action OnExitPressed;

    /// <summary>
    /// Called when "Menu" button in game result panel pressed
    /// </summary>
    event Action OnGameResultToMenuPressed;

    /// <summary>
    /// Called when "Save" button in game result panel pressed
    /// </summary>
    event Action OnGameResultSavePressed;

    /// <summary>
    /// Currently enabled panel
    /// </summary>
    UIMenuPanel CurrentPanel { get; }

    /// <summary>
    /// Enable panel. Disable all other panels
    /// <param name="panel">Panel to enable</param>
    /// </summary>
    void ShowPanel(UIMenuPanel panel);

    /// <summary>
    /// Enable or disable buttons on game result panel
    /// </summary>
    void ShowGameResultButtons(bool show);

    /// <summary>
    /// Set a value of pre-game countdown
    /// </summary>
    void SetCountdownCounter(int counter);

    /// <summary>
    /// Set game session summary on "Game Result" panel
    /// <param name="numQuestionsTotal">Total count of questions in a session</param>
    /// <param name="numCorrectQuestions">Number of correc answered questions</param>
    /// <param name="timeSeconds">Time of a session in seconds</param>
    /// </summary>
    void SetGameResult(int numQuestionsTotal, int numCorrectQuestions, double timeSeconds);

    /// <summary>
    /// Set a value of in-game timer
    /// </summary>
    void SetGameTimer(double seconds);

    /// <summary>
    /// Set question data on "Quiz" panel
    /// </summary>
    void SetQuestionData(QuestionDataModel questionData);
}
