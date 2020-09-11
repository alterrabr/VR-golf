using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine;

/// <summary>
/// This class is used to load Quiz data from StreamingAssets folder
/// </summary>
public class JSONQuizDataLoader : IQuizDataLoader
{
    private const string QuizFileName = "/Quiz.json";
    private const string QuizFolder = "/Quiz";

    /// <summary>
    /// This method will be load quiz data from JSON file in StreaminAssets folder 
    /// </summary>
    /// <param name="onLoadingComplete">Method, that will set question sequence data from file (for successful loading)</param>
    /// <param name="onLoadingFailed">Method, that will set question sequence to default value (for failed loading)</param>
    public void LoadQuiz(Action<QuizDataModel> onLoadingComplete, Action onLoadingFailed)
    {
        var quiz = new QuizDataModel();

        string dir = Path.Combine(Application.streamingAssetsPath + QuizFolder);

        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            onLoadingFailed?.Invoke();
        }

        string path = Path.Combine(dir + QuizFileName);

        if (!File.Exists(path))
        {
            onLoadingFailed?.Invoke();
        }

        using (StreamReader file = File.OpenText(path))
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                quiz = (QuizDataModel)serializer.Deserialize(file, typeof(QuizDataModel));
                onLoadingComplete?.Invoke(quiz);
            }
            catch (JsonSerializationException e)
            {
                onLoadingFailed?.Invoke();
            }

            if (quiz.Questions == null || quiz.Questions.Count == 0)
            {
                onLoadingFailed?.Invoke();
            }
        }
    }
}
