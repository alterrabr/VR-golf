using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to control keyboard configuration. It also gives events, that invoked when user clicks the buttons
/// </summary>
public class UIKeyboardController : MonoBehaviour, IUIKeyboardController
{
    [SerializeField]
    [Tooltip("The canvas that all panels")]
    private GameObject keyboardCanvas;

    [SerializeField]
    [Tooltip("The panel that contains all keys and name input field")]
    private GameObject keyboardPanel;

    [SerializeField]
    [Tooltip("The panel, that opens when user try save empty name")]
    private GameObject emptyNamePanel;

    [SerializeField]
    [Tooltip("The panel, that opens when user try save name, that already exists in the leaders" +
        "board and existing result is better than user's result")]
    private GameObject dialoguePanel;

    [SerializeField]
    [Tooltip("Input field for user name")]
    private InputField nameInputField;

    /// <summary>
    /// It is called when the user clicks the "Submit" button on the keyboard panel
    /// </summary>
    public event Action OnSubmit;

    /// <summary>
    /// It is called when the user clicks the "Cancel" button on the keyboard panel
    /// </summary>
    public event Action OnCancel;

    /// <summary>
    /// It is called when the user clicks the "Insert my name" button on the dialogue panel
    /// </summary>
    public event Action OnInsertUserNameClick;

    /// <summary>
    /// It is called when the user clicks the "Change name" button on the dialogue panel
    /// </summary>
    public event Action OnChangeNameClick;

    /// <summary>
    /// It is called when the user clicks the "Ok" button on the empty name panel
    /// </summary>
    public event Action OnEmptyNameOkClick;

    public string NameText
    {
        get
        {
            return nameInputField.text;
        }
        set
        {
            nameInputField.text = value;
        }
    }

    public void OpenDialoguePanel()
    {
        keyboardCanvas.SetActive(true);
        keyboardPanel.SetActive(false);
        emptyNamePanel.SetActive(false);
        dialoguePanel.SetActive(true);
    }

    public void OpenEmptyNamePanel()
    {
        keyboardCanvas.SetActive(true);
        keyboardPanel.SetActive(false);
        emptyNamePanel.SetActive(true);
        dialoguePanel.SetActive(false);
    }

    public void OpenKeyboardPanel()
    {
        keyboardCanvas.SetActive(true);
        keyboardPanel.SetActive(true);
        emptyNamePanel.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    public void CloseAllKeyboardPanels()
    {
        keyboardPanel.SetActive(false);
        emptyNamePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        keyboardCanvas.SetActive(false);
    }


    /// <summary>
    /// Add this method to OnClick event of Keyboard "Submit" button
    /// </summary>
    public void OnBtnSubmitClick() => OnSubmit?.Invoke();

    /// <summary>
    /// Add this method to OnClick event of Keyboard "Cancel" button
    /// </summary>
    public void OnBtnCancelClick() => OnCancel?.Invoke();

    /// <summary>
    /// Add this method to OnClick event of dialoguePanel "Insert my result" button
    /// </summary>
    public void OnBtnInsertUserNameClick() => OnInsertUserNameClick?.Invoke();

    /// <summary>
    /// Add this method to OnClick event of dialoguePanel "Change name" button
    /// </summary>
    public void OnBtnChangeNameClick() => OnChangeNameClick?.Invoke();

    /// <summary>
    /// Add this method to OnClick event of EmptyNamePanel "OK" button
    /// </summary>
    public void OnBtnEmptyNameOkClick() => OnEmptyNameOkClick?.Invoke();

    private void OnDestroy()
    {
        OnCancel = OnSubmit = OnEmptyNameOkClick = OnInsertUserNameClick = OnChangeNameClick = null;
    }
}
