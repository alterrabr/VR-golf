using UnityEngine;
using System;

/// <summary>
/// Interface that provides event for ball-hole collision
/// </summary>
public interface IAnswerHoleController
{
    /// <summary>
    /// Called when ball collides with this hole
    /// </summary>
    event Action OnCollisionWithBall;
}
