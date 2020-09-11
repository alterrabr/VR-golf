using System;

/// <summary>
/// This interface contains the basic logic for VR world space keyboard, which the player uses to add the result to the leadersboard.
/// </summary>
public interface IUIKeyboardController
{
    /// <summary>
    /// This text is used in the keyboard input field. When a player enters a name, this text changes.
    /// If this text changes, the text in the keyboard input field will also change.
    /// </summary>
    string NameText { get; set; }

    /// <summary>
    /// It is called when the user clicks the "Submit" button on the keyboard panel
    /// </summary>
    event Action OnSubmit;

    /// <summary>
    /// It is called when the user clicks the "Cancel" button on the keyboard panel
    /// </summary>
    event Action OnCancel;

    /// <summary>
    /// It is called when the user clicks the "Insert my name" button on the dialogue panel
    /// </summary>
    event Action OnInsertUserNameClick;

    /// <summary>
    /// It is called when the user clicks the "Change name" button on the dialogue panel
    /// </summary>
    event Action OnChangeNameClick;

    /// <summary>
    /// It is called when the user clicks the "Ok" button on the empty name panel
    /// </summary>
    event Action OnEmptyNameOkClick;

    /// <summary>
    /// If the user try add record for name, that already exists on the leaderboard,
    /// you can use this method to show the panel, that the user can use to choice of action
    /// </summary>
    void OpenDialoguePanel();

    /// <summary>
    /// If the user try add record for empty name,
    /// you can use this method to show the panel that notifies the user about this
    /// </summary>
    void OpenEmptyNamePanel();

    /// <summary>
    /// You can use this method to show the panel that user can use ti input the player name
    /// </summary>
    void OpenKeyboardPanel();

    /// <summary>
    /// Close keyboard panel, empty name panel and dialogue panel
    /// </summary>
    void CloseAllKeyboardPanels();
}
