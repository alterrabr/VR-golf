using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manages all GameStateManager dependencies
/// </summary>
public class DependencyManager : MonoBehaviour
{
    [Header("Game state manager")]
    [SerializeField]
    [Tooltip("Game state manager")]
    private GameStateManager gameStateManager;

    [Header("Controllers")]
    [SerializeField]
    [Tooltip("Reference to a VRTK left controller wrapper")]
    private VRTK_HandController vrtkLeftController;

    [SerializeField]
    [Tooltip("Reference to a VRTK right controller wrapper")]
    private VRTK_HandController vrtkRightController;

    [Header("Game UI controller")]
    [SerializeField]
    [Tooltip("Reference to UI menu controller")]
    private UIMenuController menuController;

    [Header("Keyboard")]
    [SerializeField]
    [Tooltip("Reference to UI keyboard controller")]
    private UIKeyboardController keyboardController;

    [Header("Scoreboard")]
    [SerializeField]
    [Tooltip("Reference to UI scoreboard controller")]
    private UIScoreBoardController scoreBoardController;

    [Header("Sound")]
    [SerializeField]
    [Tooltip("Reference to a sound manager")]
    private SoundManager soundManager;

    [Header("Teleportation")]
    [SerializeField]
    [Tooltip("Reference to a wrapper of VRTK teleport logic")]
    private VRTK_TeleportController teleportController;

    [Header("Hole controllers")]
    [SerializeField]
    [Tooltip("Collection of answer hole controllers")]
    private List<AnswerHoleController> holeControllers;

    [SerializeField]
    [Tooltip("Hole controller for an easter egg")]
    private AnswerHoleController easterEggHoleController;


    private void Awake()
    {
        // Convert list of controllers to list of interfaces to controllers
        var holeControllerInterfaces = holeControllers.ConvertAll<IAnswerHoleController>((input) => input);

        // Setup confog for GameStateManager
        var config = new GameStateManagerConfig()
        {
            RightController = new RightHandController(vrtkRightController),
            LeftController = new LeftHandController(vrtkLeftController),
            MenuController = menuController,
            KeyboardController = keyboardController,
            ScoreBoardController = scoreBoardController,
            SoundManager = soundManager,
            TeleportController = teleportController,
            ScoreDataProvider = new JSONDataLoader(),
            QuizDataProvider = new FirebaseQuizDataLoader(),   //or JSONQuizDataLoader
            HoleControllers = holeControllerInterfaces,
            EasterEggHoleController = easterEggHoleController,
        };

        gameStateManager.Initialize(config);
    }

    private void OnDestroy()
    {
        gameStateManager.UnsubscribeEvents();
    }
}
