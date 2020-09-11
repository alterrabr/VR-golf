using System.Collections.Generic;
using System;

/// <summary>
/// An interface for any implementation of data provider for quiz;
/// </summary>
public interface IQuizDataLoader
{
    /// <summary>
    /// Load a quiz data pack
    /// </summary>
    /// <returns>A set of questions, the time of the game session, and the number of questions in the game session</returns>
    void LoadQuiz(Action<QuizDataModel> onLoadingComplete, Action onLoadingFailed);
}
