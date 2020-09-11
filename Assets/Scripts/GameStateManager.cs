using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Configuration of GameStateManager class. Includes all GameStateManager dependencies
/// </summary>
public struct GameStateManagerConfig
{
    /// <summary>
    /// Right hand controller
    /// </summary>
    public HandController RightController;

    /// <summary>
    /// Left hand controller
    /// </summary>
    public HandController LeftController;

    /// <summary>
    /// Menu UI controller interface
    /// </summary>
    public IUIMenuController MenuController;

    /// <summary>
    /// Keyboard UI controller interface
    /// </summary>
    public IUIKeyboardController KeyboardController;

    /// <summary>
    /// Score UI controller interface
    /// </summary>
    public IUIScoreBoardController ScoreBoardController;

    /// <summary>
    /// Sound manager interface
    /// </summary>
    public ISoundManager SoundManager;

    /// <summary>
    /// Teleport controller interface
    /// </summary>
    public ITeleportController TeleportController;

    /// <summary>
    /// Data provider for score
    /// </summary>
    public IDataLoader ScoreDataProvider;

    /// <summary>
    /// Data provider for quiz
    /// </summary>
    public IQuizDataLoader QuizDataProvider;

    /// <summary>
    /// A collection of answer hole controllers
    /// </summary>
    public List<IAnswerHoleController> HoleControllers;

    /// <summary>
    /// Hole controller for an easter egg
    /// </summary>
    public IAnswerHoleController EasterEggHoleController;
}

/// <summary>
/// Main class of the game. Responsible for game logic and game state management
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private IUIMenuController menuController;

    private IUIScoreBoardController scoreBoardController;

    private IUIKeyboardController keyboardController;

    private List<IAnswerHoleController> holeControllers;

    private IAnswerHoleController easterEggHoleController;

    private ISoundManager soundManager;

    private ITeleportController teleportController;

    private HandController leftController;

    private HandController rightController;

    private IQuizDataLoader quizDataProvider;

    private IDataLoader scoreDataProvider;

    private KeyboardHelper keyboard;

    private ScoreBoardHelper scoreBoard;

    private ControllerManager controllerManager;

    private QuestionSequence questionSequence;

    private bool inGame;

    private float currentGameCountdown;

    private float gameSessionDurationSeconds = 5.0f * 60.0f;

    private float gameSessionTimer;

    private List<Action> eventUnsubscribers = new List<Action>();

    [Header("Timers")]
    [Tooltip("Countdown before the game starts")]
    [SerializeField]
    private float gameStartCountdown = 3.0f;

    [SerializeField]
    [Tooltip("Threshold before final countdown starts")]
    private int finalCountdownThresholdSeconds = 3;

    [Header("Default position")]
    [SerializeField]
    [Tooltip("A position for teleporting player when game session ends")]
    private Transform playerDefaultPosition;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Enable debug capabilities")]
    private bool debugMode;


    /// <summary>
    /// Intialize GameStateManager with dependencies passed in config
    /// </summary>
    public void Initialize(GameStateManagerConfig config)
    {
        leftController = config.LeftController;
        rightController = config.RightController;
        menuController = config.MenuController;
        keyboardController = config.KeyboardController;
        scoreBoardController = config.ScoreBoardController;
        soundManager = config.SoundManager;
        teleportController = config.TeleportController;
        scoreDataProvider = config.ScoreDataProvider;
        quizDataProvider = config.QuizDataProvider;
        holeControllers = config.HoleControllers;
        easterEggHoleController = config.EasterEggHoleController;
    }

    /// <summary>
    /// Unsubscribe all events to which GameStateManager was subscribed
    /// </summary>
    public void UnsubscribeEvents()
    {
        foreach (var it in eventUnsubscribers)
        {
            it.Invoke();
        }

        eventUnsubscribers.Clear();

        keyboard.UnsubscribeEvents();
        controllerManager.UnsubscribeEvents();
        leftController.UnsubscribeEvents();
        rightController.UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        // Subscribe to all events and add unsubscribers for further unsubscription
        // TODO: We need automate the way we add unsubscribers
        menuController.OnMainMenuStartGamePressed += MainMenuToHelpTransition;

        menuController.OnMainMenuHelpPressed += MainMenuToHelpTransition;
        eventUnsubscribers.Add(() => menuController.OnMainMenuHelpPressed -= MainMenuToHelpTransition);

        menuController.OnMainMenuScoreTablePressed += MainMenuToScoreTransition;
        eventUnsubscribers.Add(() => menuController.OnMainMenuScoreTablePressed -= MainMenuToScoreTransition);

        menuController.OnHelpSubmitPressed += HelpToCountdownTransition;
        eventUnsubscribers.Add(() => menuController.OnHelpSubmitPressed -= HelpToCountdownTransition);

        menuController.OnHelpCancelPressed += HelpToMainMenuTransition;
        eventUnsubscribers.Add(() => menuController.OnHelpCancelPressed -= HelpToMainMenuTransition);

        menuController.OnQuizStopGamePressed += InGameToMainMenuTransition;
        eventUnsubscribers.Add(() => menuController.OnQuizStopGamePressed -= InGameToMainMenuTransition);

        scoreBoardController.OnCancelPressed += ScoreToMainMenuTransition;
        eventUnsubscribers.Add(() => scoreBoardController.OnCancelPressed -= ScoreToMainMenuTransition);

        menuController.OnExitPressed += ExitGame;
        eventUnsubscribers.Add(() => menuController.OnExitPressed -= ExitGame);

        menuController.OnGameResultToMenuPressed += GameResultToMainMenuTransition;
        eventUnsubscribers.Add(() => menuController.OnGameResultToMenuPressed -= GameResultToMainMenuTransition);

        menuController.OnGameResultSavePressed += GameResultToKeyboardTransition;
        eventUnsubscribers.Add(() => menuController.OnGameResultSavePressed -= GameResultToKeyboardTransition);

        Action onLeftGrab = () => { leftController.SetControllerMode(HandController.Mode.Grab); };
        leftController.OnGrab += onLeftGrab;
        eventUnsubscribers.Add(() => leftController.OnGrab -= onLeftGrab);

        Action onLeftUngrab = () => { leftController.SetControllerMode(HandController.Mode.Game); };
        leftController.OnUngrab += onLeftUngrab;
        eventUnsubscribers.Add(() => leftController.OnUngrab -= onLeftUngrab);

        Action onRightGrab = () => { rightController.SetControllerMode(HandController.Mode.Grab); };
        rightController.OnGrab += onRightGrab;
        eventUnsubscribers.Add(() => rightController.OnGrab -= onRightGrab);

        Action onRightUngrab = () => { rightController.SetControllerMode(HandController.Mode.Game); };
        rightController.OnUngrab += onRightUngrab;
        eventUnsubscribers.Add(() => rightController.OnUngrab -= onRightUngrab);

        keyboard.OnKeyboardWantClose += KeyboardToGameResultTransitiion;
        eventUnsubscribers.Add(() => keyboard.OnKeyboardWantClose -= KeyboardToGameResultTransitiion);

        Action<string> onAddNewEntry = (name) => GameResultToScoreTransition(name);
        keyboard.OnAddNewEntry += onAddNewEntry;
        eventUnsubscribers.Add(() => keyboard.OnAddNewEntry -= onAddNewEntry);

        easterEggHoleController.OnCollisionWithBall += OnBallHitEasterEgg;
        eventUnsubscribers.Add(() => easterEggHoleController.OnCollisionWithBall -= OnBallHitEasterEgg);

        for (int i = 0; i < holeControllers.Count; i++)
        {
            var controller = holeControllers[i];
            int holeNum = i + 1;
            Action onCollisionWithBall = () => OnHoleCollidedWithBall(holeNum);
            controller.OnCollisionWithBall += onCollisionWithBall;
            eventUnsubscribers.Add(() => controller.OnCollisionWithBall -= onCollisionWithBall);
        }
    }

    private void OnValidate()
    {
        // Clamp finalCountdownThresholdSeconds to correct value
        finalCountdownThresholdSeconds = Math.Min(finalCountdownThresholdSeconds, (int)gameSessionDurationSeconds);
        finalCountdownThresholdSeconds = Math.Max(finalCountdownThresholdSeconds, 0);
    }

    private void Start()
    {
        // Create helpers
        keyboard = new KeyboardHelper(keyboardController);
        scoreBoard = new ScoreBoardHelper(scoreBoardController);
        controllerManager = new ControllerManager(this, rightController, leftController);

        quizDataProvider.LoadQuiz(SetQuestionSequence, ()=> SetQuestionSequence(QuizDataModel.GetDefaultQuizSet()));

        // Continue the game anyway. If questions were not loaded the game will ended just after start

        SubscribeEvents();
        InitialSetup();
    }

    private void Update()
    {
        if (debugMode)
        {
            if (inGame)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    bool correct = questionSequence.SubmitAnswer(questionSequence.GetCurrentQuestion().NumberOfCorrectAnswer);
                    if (correct)
                    {
                        soundManager.PlaySound(GameSound.CorrectAnswer);
                        QuestionDataModel question;
                        bool get = questionSequence.GetNextQuestion(out question);

                        if (get)
                        {
                            menuController.SetQuestionData(question);
                        }
                        else
                        {
                            // Cancel game if there is no questions
                            InGameToGameResultTransition();
                        }
                    }
                }
            }
        }
    }

    private void InitialSetup()
    {
        soundManager.PlayMainTheme();

        menuController.ShowPanel(UIMenuPanel.MainMenu);

        leftController.SetControllerMode(HandController.Mode.Menu);
        rightController.SetControllerMode(HandController.Mode.Menu);
    }

    /// <summary>
    /// Return to main menu from game result
    /// </summary>
    private void GameResultToMainMenuTransition()
    {
        keyboard.Hide();
        menuController.ShowPanel(UIMenuPanel.MainMenu);
    }

    /// <summary>
    /// Transition that opens keyboard and disabled buttons in game result panel
    /// </summary>
    private void GameResultToKeyboardTransition()
    {
        keyboard.Show();
        menuController.ShowGameResultButtons(false);
    }

    /// <summary>
    /// Transition that closes keyboard and enables buttons in game result panel
    /// </summary>
    private void KeyboardToGameResultTransitiion()
    {
        keyboard.Hide();
        menuController.ShowGameResultButtons(true);
    }

    /// <summary>
    /// Transition from game result state to scoreboard. Should be called when user closes the keyboard
    /// </summary>
    private void GameResultToScoreTransition(string entryToAdd)
    {
        menuController.ShowPanel(UIMenuPanel.None);
        keyboard.Hide();

        scoreBoard.SetLoadingTooltip();
        scoreBoard.ShowBoard(true);

        var scrollMode = ScoreBoardHelper.ScrollMode.ResetToBegin;

        if (entryToAdd != null)
        {
            scoreDataProvider.SaveScoreEntryAsync(MakeScoreBoardEntry(entryToAdd), () =>
            {
                scoreDataProvider.LoadScoreDataAsync((sender, args) =>
                {
                    scoreBoard.RefreshBoard(args.data);
                    scoreBoard.ScrollBoard(scrollMode);
                });
            });
        }
        else
        {
            scoreDataProvider.LoadScoreDataAsync((sender, args) =>
            {
                scoreBoard.RefreshBoard(args.data);
                scoreBoard.ScrollBoard(scrollMode);
            });
        }
    }

    private PlayerDataModel MakeScoreBoardEntry(string name)
    {
        // Collect entry data
        float sessionTime = gameSessionDurationSeconds - Mathf.Max(0.0f, gameSessionTimer);

        var quizResult = questionSequence.GetResult();
        int correctAnswers = quizResult.CorrectAswers;

        // Add entry to the database
        return new PlayerDataModel(name, correctAnswers, sessionTime);
    }

    /// <summary>
    /// Transition main menu to the help panel
    /// </summary>
    private void MainMenuToHelpTransition()
    {
        menuController.ShowPanel(UIMenuPanel.Help);
    }

    /// <summary>
    /// Transition help panel to main menu
    /// </summary>
    private void HelpToMainMenuTransition()
    {
        menuController.ShowPanel(UIMenuPanel.MainMenu);
    }

    /// <summary>
    /// Transition from main menu to pre-game countdown
    /// </summary>
    private void HelpToCountdownTransition()
    {
        menuController.ShowPanel(UIMenuPanel.Countdown);
        currentGameCountdown = gameStartCountdown;
        StartCoroutine(CountdownCoroutine());
    }

    /// <summary>
    /// Transition from game session main menu. Should be called when user cancel the game
    /// </summary>
    private void InGameToMainMenuTransition()
    {
        leftController.SetControllerMode(HandController.Mode.Menu);
        rightController.SetControllerMode(HandController.Mode.Menu);

        StopCoroutine("InGameTimerCoroutine");

        menuController.ShowPanel(UIMenuPanel.MainMenu);
    }

    /// <summary>
    /// Transition main menu to scoreboard
    /// </summary>
    private void MainMenuToScoreTransition()
    {
        menuController.ShowPanel(UIMenuPanel.None);
        scoreBoard.ClearBoard();

        scoreBoard.SetLoadingTooltip();
        scoreBoard.ShowBoard(true);

        scoreDataProvider.LoadScoreDataAsync((sender, args) =>
        {
            if (args.status == ScoreLoadingStatus.Ok)
            {
                scoreBoard.RefreshBoard(args.data);
                scoreBoard.ScrollBoard(ScoreBoardHelper.ScrollMode.ResetToBegin);
                // TODO: Save

            }
            else
            {
                scoreBoard.ClearBoard();
            }
        });
    }

    /// <summary>
    /// Transitiion from pre-game countdown to game session
    /// </summary>
    private void CountdownToInGameTransition()
    {
        inGame = true;

        leftController.SetControllerMode(HandController.Mode.Game);
        rightController.SetControllerMode(HandController.Mode.Game);

        menuController.ShowPanel(UIMenuPanel.Quiz);

        questionSequence.StartNewSequence();

        QuestionDataModel question;
        bool get = questionSequence.GetNextQuestion(out question);

        if (get)
        {
            menuController.SetQuestionData(question);

            gameSessionTimer = gameSessionDurationSeconds;
            menuController.SetGameTimer(gameSessionTimer);

            // Starting coroutine by name because StopCoroutine does not work when it
            // was started by function itsef (StartCoroutine(InGameTimerCoroutine()))
            StartCoroutine("InGameTimerCoroutine");

        }
        else
        {
            // Cancel game if there is no questions
            InGameToGameResultTransition();
        }
    }

    /// <summary>
    /// Transition scoreboard to main menu
    /// </summary>
    private void ScoreToMainMenuTransition()
    {
        scoreBoard.ShowBoard(false);
        menuController.ShowPanel(UIMenuPanel.MainMenu);
    }

    /// <summary>
    /// Transition from game session to game result. Called when game timer reached end,  or when user answers all questions
    /// </summary>
    private void InGameToGameResultTransition()
    {
        inGame = false;

        // Ungrabbing all grabbed item before change state.
        // This way we will not trigger ungrab events in the middle of state change

        // TODO: When we will implement better controller logic we should not send ungrab events
        // when controller interactions are disabled
        leftController.Ungrab();
        rightController.Ungrab();

        leftController.SetControllerMode(HandController.Mode.Menu);
        rightController.SetControllerMode(HandController.Mode.Menu);

        menuController.ShowPanel(UIMenuPanel.GameResult);

        menuController.ShowGameResultButtons(true);

        soundManager.PlaySound(GameSound.GameEnd);

        var quizResult = questionSequence.GetResult();

        float sessionTime = gameSessionDurationSeconds - Mathf.Max(0.0f, gameSessionTimer);

        menuController.SetGameResult(quizResult.QuestionCount, quizResult.CorrectAswers, sessionTime);

        //keyboard.Show();

        StopCoroutine("InGameTimerCoroutine");

        // Teleport player to a special position new menu
        teleportController.Teleport(playerDefaultPosition.position, playerDefaultPosition.forward);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Callback for hole collison with ball
    /// </summary>
    private void OnHoleCollidedWithBall(int holeNumber)
    {
        Debug.Log("Game manager: The ball has collided with hole " + holeNumber.ToString());

        if (inGame)
        {
            bool correct = questionSequence.SubmitAnswer(holeNumber);
            if (correct)
            {
                soundManager.PlaySound(GameSound.CorrectAnswer);
                QuestionDataModel question;
                bool get = questionSequence.GetNextQuestion(out question);

                if (get)
                {
                    menuController.SetQuestionData(question);
                }
                else
                {
                    // Cancel game if there is no questions
                    InGameToGameResultTransition();
                }
            }
            else
            {
                soundManager.PlaySound(GameSound.WrongAnswer);
            }
        }
    }

    private void OnBallHitEasterEgg()
    {
        soundManager.PlaySound(GameSound.EasterEgg);
    }

    private IEnumerator InGameTimerCoroutine()
    {
        // Set count to negative value in order t disable countdown when finalCountdownThresholdSeconds is set to 0
        int count = finalCountdownThresholdSeconds > 0 ? finalCountdownThresholdSeconds : -1;

        while (true)
        {
            gameSessionTimer -= Time.deltaTime;

            menuController.SetGameTimer(gameSessionTimer);

            if (gameSessionTimer <= 0.0f)
            {
                InGameToGameResultTransition();
                break;
            }

            if ((int)gameSessionTimer == count)
            {
                count--;
                soundManager.PlaySound(GameSound.Countdown);
            }

            yield return null;
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        int count = (int)gameStartCountdown;
        while (true)
        {
            currentGameCountdown -= Time.deltaTime;
            menuController.SetCountdownCounter(((int)currentGameCountdown + 1));

            if ((int)(currentGameCountdown + 1) == count)
            {
                if (count == 0)
                {
                    soundManager.PlaySound(GameSound.GameStart);
                }
                else
                {
                    soundManager.PlaySound(GameSound.Countdown);
                }
                count--;
            }

            if (currentGameCountdown <= 0.0f)
            {
                CountdownToInGameTransition();
                break;
            }

            yield return null;
        }
    }

    private void SetQuestionSequence(QuizDataModel model) => questionSequence = new QuestionSequence(model);
}
