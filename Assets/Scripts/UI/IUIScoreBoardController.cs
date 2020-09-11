using System;
using System.Collections.Generic;

/// <summary>
/// An interface that defines scoreboard UI public methods and fields
/// </summary>
public interface IUIScoreBoardController
{
    /// <summary>
    /// This action is called when scoreboard "Close" button is pressed
    /// </summary>
    event Action OnCancelPressed;

    /// <summary>
    /// Speed of a scroll effect
    /// </summary>
    float ScrollSpeed { get; set; }

    /// <summary>
    /// Current state of a scoreboard
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// Enable or disable scoreboard
    /// </summary>
    void ShowBoard(bool show);

    /// <summary>
    /// Enable score loading tooltip
    /// </summary>
    void SetLoadingTooltip();

    /// <summary>
    /// Set scroll position
    /// </summary>
    void SetScroll(float normalizedPosition);

    /// <summary>
    /// Do smooth scroll to specified position
    /// </summary>
    void ScrollTo(float normalizedPosition);

    /// <summary>
    /// Set scoreboard
    /// </summary>
    void SetData(List<PlayerDataModel> data);

    /// <summary>
    /// Remove all records from scoreboard
    /// </summary>
    void ClearBoard();
}
