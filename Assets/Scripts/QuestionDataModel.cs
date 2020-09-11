using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data model for the question
/// </summary>
[Serializable]
public struct QuestionDataModel
{
    /// <summary>
    /// Quiz Question
    /// </summary>
    public string Question;

    /// <summary>
    /// Quiz answer number 1
    /// </summary>
    public string Answer1;

    /// <summary>
    /// Quiz answer number 2
    /// </summary>
    public string Answer2;

    /// <summary>
    /// Quiz answer number 3
    /// </summary>
    public string Answer3;

    /// <summary>
    /// Number of the right Quiz answer
    /// </summary>
    public int NumberOfCorrectAnswer;
}

[Serializable]
public struct QuizDataModel
{
    /// <summary>
    /// Number of questions for one game sessions
    /// </summary>
    public int NumberOfQuestionsForSession;

    /// <summary>
    /// Time of one game session in seconds
    /// </summary>
    public int RoundTime;

    /// <summary>
    /// A collection of questions
    /// </summary>
    public IList<QuestionDataModel> Questions;

    /// <summary>
    /// This method is used to prepare the default quiz set: 4 arithmetic questions in 5 minutes
    /// </summary>
    /// <returns></returns>
    public static QuizDataModel GetDefaultQuizSet()
    {
        var defaultQuizSet = new QuizDataModel();
        defaultQuizSet.NumberOfQuestionsForSession = 4;
        defaultQuizSet.RoundTime = 300;

        defaultQuizSet.Questions.Add(new QuestionDataModel() { Question = "2+2*2", Answer1 = "4", Answer2 = "6", Answer3 = "8", NumberOfCorrectAnswer = 2 });
        defaultQuizSet.Questions.Add(new QuestionDataModel() { Question = "(2+2)*2", Answer1 = "4", Answer2 = "6", Answer3 = "8", NumberOfCorrectAnswer = 3 });
        defaultQuizSet.Questions.Add(new QuestionDataModel() { Question = "2+2*2/2", Answer1 = "4", Answer2 = "6", Answer3 = "8", NumberOfCorrectAnswer = 1 });
        defaultQuizSet.Questions.Add(new QuestionDataModel() { Question = "(2+2)*2/2", Answer1 = "4", Answer2 = "6", Answer3 = "8", NumberOfCorrectAnswer = 1 });

        return defaultQuizSet;
    }
}