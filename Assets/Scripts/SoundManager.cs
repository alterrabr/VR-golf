using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// This class manages all in-game audio clips and audio sources
/// </summary>
public class SoundManager : MonoBehaviour, ISoundManager
{
    [Header("Sounds")]
    [SerializeField]
    [Tooltip("This sound played when player hits correct hole")]
    private AudioClip correctAnswerSound;

    [SerializeField]
    [Tooltip("This sound played when player hits wrong hole")]
    private AudioClip wrongAnswerSound;

    [SerializeField]
    [Tooltip("This sound played when countdown before the game end is started")]
    private AudioClip countdownSound;

    [SerializeField]
    [Tooltip("This sound played when the game ends")]
    private AudioClip gameEndSound;

    [SerializeField]
    [Tooltip("This sound played when the game starts")]
    private AudioClip gameStartSound;

    [SerializeField]
    [Tooltip("This sound played when easter egg gets hit")]
    private AudioClip easterEggSound;

    [SerializeField]
    [Tooltip("Main theme of the game")]
    private AudioClip mainThemeSound;

    [Header("Audio sources")]
    [SerializeField]
    [Tooltip("Source for a main theme sound")]
    private AudioSource mainThemeAudioSource;

    [SerializeField]
    [Tooltip("Source which is used for playing \"one shot\" sounds")]
    private AudioSource oneShotAudioSource;

   /// <summary>
   /// Start playing game main theme
   /// </summary>
    public void PlayMainTheme()
    {
        mainThemeAudioSource.clip = mainThemeSound;
        mainThemeAudioSource.loop = true;
        mainThemeAudioSource.Play(0);
    }

   /// <summary>
   /// Play a particular sound once
   /// </summary>
    public void PlaySound(GameSound sound)
    {
        AudioClip clip = null;
        switch (sound)
        {
            case GameSound.CorrectAnswer: { clip = correctAnswerSound; } break;
            case GameSound.WrongAnswer: { clip = wrongAnswerSound; } break;
            case GameSound.Countdown: { clip = countdownSound; } break;
            case GameSound.GameEnd: { clip = gameEndSound; } break;
            case GameSound.GameStart: { clip = gameStartSound; } break;
            case GameSound.EasterEgg: { clip = gameStartSound; } break;
            default: { Debug.LogError("Invalid default case"); } break;
        };

        oneShotAudioSource.PlayOneShot(clip);
    }
}
