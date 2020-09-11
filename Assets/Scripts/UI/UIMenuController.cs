using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class represents game menu UI. Implements IUIMenuController
/// </summary>
public class UIMenuController : MonoBehaviour, IUIMenuController
{
    [Header("Main menu UI")]
    [SerializeField]
    [Tooltip("Main menu panel")]
    private GameObject grp_MainMenu;

    [Header("Quiz UI")]
    [SerializeField]
    [Tooltip("Panel with questions and answers")]
    private GameObject grp_Quiz;

    [SerializeField]
    [Tooltip("Question UI text")]
    private Text txt_Question;

    [SerializeField]
    [Tooltip("Answer 1 UI text")]
    private Text txt_Answer1;

    [SerializeField]
    [Tooltip("Answer 2 UI text")]
    private Text txt_Answer2;

    [SerializeField]
    [Tooltip("Answer 3 UI text")]
    private Text txt_Answer3;

    [SerializeField]
    [Tooltip("Counter for game session timer")]
    private Text txt_TimerCounter;

    [Header("Help UI")]
    [SerializeField]
    [Tooltip("Panel with help")]
    private GameObject grp_Help;

    [Header("Countdown UI")]
    [SerializeField]
    [Tooltip("Countdown panel")]
    private GameObject grp_Countdown;

    [SerializeField]
    [Tooltip("Countdown counter text")]
    private Text txt_Counter;

    [Header("Game result UI")]
    [SerializeField]
    [Tooltip("Right answer counter")]
    private GameObject grp_GameResult;

    [SerializeField]
    [Tooltip("Right answer counter")]
    private Text txt_RightAnswers;

    [SerializeField]
    [Tooltip("Game session time display")]
    private Text txt_Time;

    [SerializeField]
    [Tooltip("Menu button")]
    private GameObject btn_ToMainMenu;

    [SerializeField]
    [Tooltip("Save button")]
    private GameObject btn_SaveResult;

    private UIMenuPanel currentPanel = UIMenuPanel.MainMenu;

    public UIMenuPanel CurrentPanel
    {
        get
        {
            return currentPanel;
        }
    }

    public event Action OnMainMenuStartGamePressed;

    public event Action OnMainMenuHelpPressed;

    public event Action OnMainMenuScoreTablePressed;

    public event Action OnHelpSubmitPressed;

    public event Action OnHelpCancelPressed;

    public event Action OnQuizStopGamePressed;

    public event Action OnExitPressed;

    public event Action OnGameResultToMenuPressed;

    public event Action OnGameResultSavePressed;

    public void ShowPanel(UIMenuPanel panel)
    {
        // TODO: For now just deactivate all panels. Maybe we should track
        // of current active panel
        grp_MainMenu.SetActive(false);
        grp_Quiz.SetActive(false);
        grp_Help.SetActive(false);
        grp_Countdown.SetActive(false);
        grp_GameResult.SetActive(false);

        currentPanel = panel;

        switch (panel)
        {
            case UIMenuPanel.None: {} break;
            case UIMenuPanel.MainMenu: { grp_MainMenu.SetActive(true); } break;
            case UIMenuPanel.Quiz: { grp_Quiz.SetActive(true); } break;
            case UIMenuPanel.Help: { grp_Help.SetActive(true); } break;
            case UIMenuPanel.Countdown: { grp_Countdown.SetActive(true); } break;
            case UIMenuPanel.GameResult: { grp_GameResult.SetActive(true); } break;
            default: { Debug.LogError("Invalid default case"); } break;
        }
    }

    public void ShowGameResultButtons(bool show)
    {
        btn_ToMainMenu.SetActive(show);
        btn_SaveResult.SetActive(show);
    }

    public void SetCountdownCounter(int value)
    {
        txt_Counter.text = value.ToString();
    }

    public void SetGameResult(int numQuestionsTotal, int numCorrectQuestions, double timeSeconds)
    {
        var timeSpan = TimeSpan.FromSeconds(timeSeconds);
        txt_Time.text = timeSpan.ToString(@"mm\:ss");
        txt_RightAnswers.text = numCorrectQuestions.ToString() + "/" + numQuestionsTotal.ToString();
    }

    public void SetGameTimer(double seconds)
    {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        txt_TimerCounter.text = timeSpan.ToString(@"mm\:ss");
    }

    public void SetQuestionData(QuestionDataModel questionData)
    {
        txt_Question.text = questionData.Question;
        txt_Answer1.text = questionData.Answer1;
        txt_Answer2.text = questionData.Answer2;
        txt_Answer3.text = questionData.Answer3;
    }

    /// <summary>
    /// Internal event handler. Should be assigned to "Start" button from inspector
    /// </summary>
    public void OnBtnMainMenuStartGamePressed()
    {
        OnMainMenuStartGamePressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should pe assigned to "Help" button from inspector
    /// </summary>
    public void OnBtnMainMenuHelpPressed()
    {
        OnMainMenuHelpPressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should pe assigned to "Score" button from inspector
    /// </summary>
    public void OnBtnMainMenuScoreTablePressed()
    {
        OnMainMenuScoreTablePressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should be assigned to "Submit" button from inspector
    /// </summary>
    public void OnBtnHelpSubmitPressed()
    {
        OnHelpSubmitPressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should be assigned to "Cancel" button from inspector
    /// </summary>
    public void OnBtnHelpCancelPressed()
    {
        OnHelpCancelPressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should be assigned to "Stop" button from inspector
    /// </summary>
    public void OnBtnQuizStopGamePressed()
    {
        OnQuizStopGamePressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should be assigned to "Stop" button from inspector
    /// </summary>
    public void OnBtnExitPressed()
    {
        OnExitPressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should be assigned to game result "Menu" button from inspector
    /// </summary>
    public void OnBtnGameResultToMenuPressed()
    {
        OnGameResultToMenuPressed?.Invoke();
    }

    /// <summary>
    /// Internal event handler. Should be assigned to game result "Menu" button from inspector
    /// </summary>
    public void OnBtnGameResultSavePressed()
    {
        OnGameResultSavePressed?.Invoke();
    }

    private void Awake()
    {
        ShowPanel(currentPanel);
    }
}