using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = System.Random;

/// <summary>
/// Helper class for managing qame quiz
/// </summary>
public class QuestionSequence
{
    /// <summary>
    /// Represents result of a game session
    /// </summary>
    public struct Result
    {
        public int QuestionCount;
        public int QuestionsAnswered;
        public int CorrectAswers;
    }

    const int DefaultNumQuestionsPerSession = 5;

    private readonly int sequenceLength;

    private readonly Random random = new Random();

    private IList<QuestionDataModel> questionPool;

    private IList<QuestionDataModel> questionCache = new List<QuestionDataModel>();

    private List<QuestionDataModel> sequence = new List<QuestionDataModel>();
    private IEnumerator<QuestionDataModel> iterator;

    //private int index;
    private int correctAnswers;
    private int questionsAnswered;

    private bool answerCurrentQuestion;

    /// <summary>
    /// Constructor for QuestionSequence
    /// </summary>
    public QuestionSequence(QuizDataModel quiz)
    {
        questionPool = quiz.Questions;
        sequenceLength = quiz.NumberOfQuestionsForSession == 0 ? DefaultNumQuestionsPerSession : quiz.NumberOfQuestionsForSession;
    }

    private void ResetPool()
    {
        foreach (var question in questionPool)
        {
            questionCache.Add(question);
        }
        questionPool.Clear();

        var temp = questionPool;
        questionPool = questionCache;
        questionCache = temp;
    }

    public void StartNewSequence()
    {
        if (questionPool.Count < sequenceLength)
        {
            ResetPool();
        }

        correctAnswers = 0;
        questionsAnswered = 0;

        sequence.Clear();

        int numQuestoins = Math.Min(questionPool.Count, sequenceLength);
        for (int i = 0; i < numQuestoins; i++)
        {
            int index = random.Next(0, questionPool.Count);
            var question = questionPool[index];
            questionPool.RemoveAt(index);
            questionCache.Add(question);
            sequence.Add(question);
        }

        iterator = sequence.GetEnumerator();
    }

    /// <summary>
    /// Submit answer to a current question
    /// <returns>Bool that indicates is answer was right</returns>
    /// </summary>
    public bool SubmitAnswer(int index)
    {
        if (!answerCurrentQuestion)
        {
            questionsAnswered++;
            Debug.Log("iterator.Current.NumberOfCorrectAnswer  " + iterator.Current.NumberOfCorrectAnswer.ToString());
            if (iterator.Current.NumberOfCorrectAnswer == index)
            {
                answerCurrentQuestion = true;
                correctAnswers++;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get next question in a sequence
    /// <returns>False if question sequence is ended</returns>
    /// </summary>
    public bool GetNextQuestion(out QuestionDataModel question)
    {
        if (iterator.MoveNext())
        {
            question = iterator.Current;
            answerCurrentQuestion = false;
            return true;
        }
        else
        {
            question = new QuestionDataModel();
            return false;
        }
    }

    public QuestionDataModel  GetCurrentQuestion()
    {
        return iterator.Current;
    }

    /// <summary>
    /// Get result for this sequence
    /// </summary>
    public Result GetResult()
    {
        return new Result()
        {
            QuestionCount = sequence.Count,
            QuestionsAnswered = questionsAnswered,
            CorrectAswers = correctAnswers
        };
    }
}
