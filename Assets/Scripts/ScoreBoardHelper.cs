using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A helper class that encapsulates scoreboard logic.
/// </summary>
public class ScoreBoardHelper
{
    private IDataLoader dataProvider;

    private IUIScoreBoardController scoreboard;

    /// <summary>
    /// Represents how scoreboard will be scrolled when enabled
    /// </summary>
    public enum ScrollMode
    {
        ResetToBegin,
        ScrollToEnd
    };

    /// <summary>
    /// Contructor of ScoreBoardHelper
    /// <param name="scoreboardController">Reference to scoreboard UI controller</param>
    /// </summary>
    public ScoreBoardHelper(IUIScoreBoardController scoreboardController)
    {
        scoreboard = scoreboardController;
    }

    /// <summary>
    /// Show score loading tooltip
    /// </summary>
    public void SetLoadingTooltip()
    {
        scoreboard.SetLoadingTooltip();
    }

    /// <summary>
    /// Refresh scoreboard data
    /// <param name="transaction">Reference to a valid transaction. Transaction will be opened it was not already</param>
    /// </summary>
    public void RefreshBoard(ScoreTransaction transaction)
    {
        // Remember current state
        var state = scoreboard.Active;
        // We need disable scoreboard before updating the data
        // Otherwise scroll will be broken
        scoreboard.ShowBoard(false);

        scoreboard.ClearBoard();
        if (transaction.HasPendingTransaction())
        {
            // TODO: This probably should be optimized in the future
            var sortedData = new List<PlayerDataModel>(transaction.Database);
            sortedData.Sort((x, y) =>
            {
                if (x.CorrectAnswers == y.CorrectAnswers)
                {
                    if (x.Time < y.Time)
                    {
                        return -1;
                    }
                    if (x.Time == y.Time)
                    {
                        return 0;
                    }
                    if (x.Time == y.Time)
                    {
                        return 1;
                    }
                }
                if (x.CorrectAnswers < y.CorrectAnswers)
                {
                    return 1;
                }
                if (x.CorrectAnswers > y.CorrectAnswers)
                {
                    return -1;
                }
                return 0;
            });

            scoreboard.SetData(sortedData);
        }
        else
        {
            Debug.LogError("Transaction is not open");
        }

        // Restore previous state
        scoreboard.ShowBoard(state);
    }

    /// <summary>
    /// Remove all data from scoreboard
    /// </summary>
    public void ClearBoard()
    {
        scoreboard.ClearBoard();
    }

    /// <summary>
    /// Enable scoreboard UI with scroll
    /// </summary>
    public void ShowBoard(bool show)
    {
        if (show && !scoreboard.Active)
        {
            scoreboard.ShowBoard(true);
        }
        else
        {
            scoreboard.ShowBoard(false);
        }
    }

    /// <summary>
    /// Scroll board with a given mode
    /// </summary>
    public void ScrollBoard(ScrollMode mode)
    {
        if (scoreboard.Active)
        {
            switch (mode)
            {
                case ScrollMode.ResetToBegin:
                {
                    scoreboard.SetScroll(1.0f);
                } break;

                case ScrollMode.ScrollToEnd:
                {
                    // Reset scroll position to begin
                    scoreboard.SetScroll(1.0f);
                    // Start smooth scrolling
                    scoreboard.ScrollTo(0.0f);
                } break;

                default:
                {
                    Debug.LogError("Invalid default case");
                } break;
            }
        }
        else
        {
            Debug.LogWarning("Srolling disabled scoreboard will have no effect");
        }

    }
}
