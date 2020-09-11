using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that represents scoreboard UI. Implements IUIScoreBoardController interface
/// </summary>
public class UIScoreBoardController : MonoBehaviour, IUIScoreBoardController
{
    [Header("Score UI")]
    [SerializeField]
    [Tooltip("Panel with scores")]
    private GameObject grp_Scores;

    [SerializeField]
    [Tooltip("")]
    private ScrollRect scoreScrollRect;

    [SerializeField]
    [Tooltip("Score string prefab")]
    private GameObject scoreStringPrefab;

    [SerializeField]
    [Tooltip("Score string loading prefab")]
    private GameObject scoreStringLoadingPrefab;


    [SerializeField]
    [Tooltip("Parent for create score string")]
    private Transform scoreStringParent;

    private float scrollDest;

    public float ScrollSpeed { get; set; } = 2.0f;

    public bool Active
    {
        get
        {
            return grp_Scores.activeSelf;
        }
    }

    public event Action OnCancelPressed;

    public void ShowBoard(bool show)
    {
        grp_Scores.SetActive(show);
    }

    public void SetScroll(float normalizedPosition)
    {
        if (grp_Scores.activeSelf)
        {
            float clamped = Mathf.Clamp(normalizedPosition, 0.0f, 1.0f);
            scoreScrollRect.verticalNormalizedPosition = clamped;
        }
    }

    public void ScrollTo(float normalizedPosition)
    {
        if (grp_Scores.activeSelf)
        {
            scrollDest = normalizedPosition;
            StopCoroutine("ScrollScoreBoardToEndCoroutine");
            StartCoroutine("ScrollScoreBoardToEndCoroutine");
        }
    }

    public void SetData(List<PlayerDataModel> data)
    {
        ClearBoard();
        foreach (var playerScore in data)
        {
            var scoreString = Instantiate(scoreStringPrefab, scoreStringParent).GetComponent<ScoreStringSetup>();
            scoreString.SetPlayerScoreData(playerScore);
        }
    }

    public void SetLoadingTooltip()
    {
        ClearBoard();
        Instantiate(scoreStringLoadingPrefab, scoreStringParent);;
    }


    public void ClearBoard()
    {
        var loadingStrings = scoreStringParent.GetComponentsInChildren<ScoreStringLoadingSetup>().ToList();
        foreach (var it in loadingStrings)
        {
            Destroy(it.gameObject);
        }

        var scoreStrings = scoreStringParent.GetComponentsInChildren<ScoreStringSetup>().ToList();
        foreach (var scoreString in scoreStrings)
        {
            scoreString.RemoveButton.onClick.RemoveAllListeners();
            Destroy(scoreString.gameObject);
        }

        scoreStrings.Clear();
    }

    private IEnumerator ScrollScoreBoardToEndCoroutine()
    {
        float dist = scrollDest - scoreScrollRect.verticalNormalizedPosition;
        float dir = Mathf.Sign(dist);
        while (Mathf.Sign(scrollDest - scoreScrollRect.verticalNormalizedPosition) == dir)
        {
            scoreScrollRect.verticalNormalizedPosition += Time.deltaTime * ScrollSpeed * dir;
            yield return null;
        }
    }

    /// <summary>
    /// Internal method. Should be assigned to "Cancel button" in inspector
    /// </summary>
    public void OnBtnScoreCancelPressed()
    {
        OnCancelPressed?.Invoke();
    }
}
