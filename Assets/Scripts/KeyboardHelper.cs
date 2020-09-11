using System;
using UnityEngine;

/// <summary>
/// Helper class that encapsulates logic of a keyboard
/// </summary>
public class KeyboardHelper
{
    /// <summary>
    /// This event called when "Close" button on a keyboard pressed
    /// </summary>
    public event Action OnKeyboardWantClose;

    /// <summary>
    /// This event called when "Submit" button on a keyboard pressed. Also passes entered string.
    /// </summary>
    public event Action<string> OnAddNewEntry;

    private IUIKeyboardController keyboardController;

    /// <summary>
    /// Constructor for KeyboardHelper
    /// </summary>
    public KeyboardHelper(IUIKeyboardController controller)
    {
        keyboardController = controller;

        keyboardController.CloseAllKeyboardPanels();
        keyboardController.NameText = "";

        keyboardController.OnSubmit += OnSubmit;
        keyboardController.OnCancel += OnCancel;
        keyboardController.OnEmptyNameOkClick += OnEmptyNameOkClick;
    }

    /// <summary>
    /// Unsubscribe all events this class was subscribed
    /// </summary>
    public void UnsubscribeEvents()
    {
        keyboardController.OnSubmit -= OnSubmit;
        keyboardController.OnCancel -= OnCancel;
        keyboardController.OnEmptyNameOkClick -= OnEmptyNameOkClick;
    }

    /// <summary>
    /// Show keyboard
    /// </summary>
    public void Show()
    {
        keyboardController.OpenKeyboardPanel();
    }

    /// <summary>
    /// Hide keyboard
    /// </summary>
    public void Hide()
    {
        keyboardController.NameText = "";
        keyboardController.CloseAllKeyboardPanels();
    }

    private void OnSubmit()
    {
        if (StringUtility.StringEmpty(keyboardController.NameText))
        {
            keyboardController.OpenEmptyNamePanel();
        }
        else
        {
            OnAddNewEntry?.Invoke(keyboardController.NameText);
        }
    }

    private void OnCancel()
    {
        OnKeyboardWantClose?.Invoke();
    }

    private void OnEmptyNameOkClick()
    {
        keyboardController.OpenKeyboardPanel();
        keyboardController.NameText = "";
    }
}

static class StringUtility
{
    public static bool StringEmpty(string str)
    {
        bool empty = true;
        foreach (char c in str)
        {
            if (!Char.IsWhiteSpace(c))
            {
                empty = false;
                break;
            }
        }
        return empty;
    }
}
