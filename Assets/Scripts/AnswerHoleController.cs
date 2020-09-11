using UnityEngine;
using System;

/// <summary>
/// Class that provides events for ball collision with hole
/// </summary>
public class AnswerHoleController : MonoBehaviour, IAnswerHoleController
{
    /// <summary>
    /// Called when a ball collides with this hole
    /// </summary>
    public event Action OnCollisionWithBall;

    private int ballLayerIndex;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == ballLayerIndex)
        {
            OnCollisionWithBall?.Invoke();
        }
    }

    private void Start()
    {
        ballLayerIndex = LayerMask.NameToLayer("Ball");
    }
}
