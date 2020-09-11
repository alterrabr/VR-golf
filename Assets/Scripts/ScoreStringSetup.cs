using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player Data Model structure
/// </summary>
[Serializable]
public class PlayerDataModel
{
    /// <summary>
    /// Player name
    /// </summary>
    public string Name;

    /// <summary>
    /// Number player correct answers
    /// </summary>
    public int CorrectAnswers;

    /// <summary>
    /// Time of game sesstion in seconds
    /// </summary>
    public float Time;

    /// <summary>
    /// Set player score
    /// </summary>
    /// <param name="name"></param>
    /// <param name="correctAnswers"></param>
    /// <param name="time"></param>
    public PlayerDataModel(string name, int correctAnswers, float time)
    {
        this.Name = name;
        this.CorrectAnswers = correctAnswers;
        this.Time = time;
    }
}

/// <summary>
/// Setup values player score string
/// </summary>
public class ScoreStringSetup : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Player name")]
    private Text txt_Name;

    [SerializeField]
    [Tooltip("Number player correct answers ")]
    private Text txt_correctAnswers;

    [SerializeField]
    [Tooltip("Player score time")]
    private Text txt_time;

    [SerializeField]
    [Tooltip("Button for removing record")]
    private Button removeButton;

    public Button RemoveButton => removeButton;

    public string UserName => txt_Name.text;

    /// <summary>
    /// Set player leader board string data
    /// </summary>
    public void SetPlayerScoreData(PlayerDataModel playerDataModel)
    {
        txt_Name.text = playerDataModel.Name;
        txt_correctAnswers.text = playerDataModel.CorrectAnswers.ToString();

        TimeSpan span = TimeSpan.FromSeconds(playerDataModel.Time);
        string time = span.ToString(@"mm\:ss");

        txt_time.text = time;
    }
}