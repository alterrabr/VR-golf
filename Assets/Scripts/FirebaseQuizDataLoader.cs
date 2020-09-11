using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;

/// <summary>
/// This class is used to load Quiz data from Firebase
/// </summary>
public class FirebaseQuizDataLoader : IQuizDataLoader
{
    private const string FirebaseQuizRootName = "Quiz";

    /// <summary>
    /// This method will be load quiz data from Firebase
    /// </summary>
    /// <param name="onLoadingComplete">Method, that will set question sequence data from file (for successful loading)</param>
    /// <param name="onLoadingFailed">Method, that will set question sequence to default value (for failed loading)</param>
    public async void LoadQuiz(Action<QuizDataModel> onLoadingComplete, Action onLoadingFailed)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        await reference.Child(FirebaseQuizRootName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                onLoadingFailed?.Invoke();
            }
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();

                try
                {
                    var quiz = JsonConvert.DeserializeObject<QuizDataModel>(json);
                    onLoadingComplete?.Invoke(quiz);
                }
                catch (JsonSerializationException e)
                {
                    onLoadingFailed?.Invoke();
                }
            }
        });
    }
}
