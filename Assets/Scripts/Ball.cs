using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball return settings")]

    [SerializeField]
    [Tooltip("Distance, upon reaching at ball will be waited speed slow down, return after that.")]
    private float ballReturnDistance = 1.5f;

    [SerializeField]
    [Tooltip("Distance, upon reaching at ball will be force returned.")]
    private float ballForceReturnDistance = 12f;

    [SerializeField]
    [Tooltip("Velocity threshold for ball return. If ball velocty less and it in return zone, then it will be return")]
    private float ballReturnVelocityThreshold = 0.2f;

    [SerializeField]
    [Tooltip("Velocity threshold for ball return from hole trigger zone.")]
    private float returnVelocityThresholdHoleZone = 0.2f;

    [SerializeField]
    [Tooltip("Countdown time before returnning the ball when it is in hole zone and it's velocity below return threshold")]
    private float returnTimeHoleZone = 1.0f;

    [SerializeField]
    [Tooltip("Countdown time before returnning the ball when it is in hole zone and it's velocity below return threshold")]
    private float returnTime = 0.5f;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Enable debug capabilities")]
    private bool debugMode = false;

    [SerializeField]
    [Tooltip("Disable ball return logic")]
    private bool disableBallReturn = false;

    // Point on scene, to where ball will be returned and created after score
    private Transform ballReturnPoint;

    // Store ball's rigidbody
    private Rigidbody ballRigidbody;

    // If ball already rolling to hole, do not return it
    private bool ballRollingToHole = false;

    private bool returnCountdownStarted;

    private float returnCountdown;

    private enum BallState
    {
        RollingToHole,
        InReturnZone,
        ForceReturn,
        InPlayZone
    };

    private BallState prevState = BallState.InPlayZone;

    private int holeLayerIndex;

    /// <summary>
    /// This action will be called, when ball enter the hole trigger
    /// </summary>
    public Action OnBallHoleZoneEnter;

    /// <summary>
    /// This action will be called, when ball exit the hole trigger
    /// </summary>
    public Action OnBallHoleZoneExit;

    public float BallReturnDistance => ballReturnDistance;

    public float BallForceReturnDistance => ballForceReturnDistance;

    /// <summary>
    /// This function stops ball's physics and movement, return to start point and toggle physics back.
    /// </summary>
    public void ForceReturnBallToReturnPoint()
    {
        // Toggle kinematic for force stop ball before teleport to start point
        ballRigidbody.isKinematic = true;

        ballRigidbody.rotation = Quaternion.identity;
        ballRigidbody.position = ballReturnPoint.position;
        ballRigidbody.isKinematic = false;

        ballRollingToHole = false;
        OnBallHoleZoneExit?.Invoke();
    }

    private void ReturnBallAndResetCountdown()
    {
        returnCountdownStarted = false;
        ForceReturnBallToReturnPoint();
    }

    /// <summary>
    /// Will return force return ball if debug mode is enabled
    /// </summary>
    public void DebugForceReturnBall()
    {
        if (debugMode)
        {
            ReturnBallAndResetCountdown();
        }
    }

    private void Start()
    {
        holeLayerIndex = LayerMask.NameToLayer("Hole");

        // Find actual instance of return point in scene
        ballReturnPoint = GameObject.FindGameObjectWithTag("BallReturnPoint").transform;

        // Store ball's rigidbody
        ballRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!disableBallReturn)
        {
            UpdateBallReturnState();
        }
    }

    private BallState GetBallState()
    {
        if (Vector3.Distance(transform.position, ballReturnPoint.position) > ballForceReturnDistance)
        {
            return BallState.ForceReturn;
        }

        if (ballRollingToHole)
        {
            return BallState.RollingToHole;
        }

        if (Vector3.Distance(transform.position, ballReturnPoint.position) > ballReturnDistance)
        {
            return BallState.InReturnZone;
        }

        return BallState.InPlayZone;
    }

    private void UpdateBallReturnState()
    {
        var state = GetBallState();

        // Return ball immediately if it in a force return zone
        if (state == BallState.ForceReturn)
        {
            ReturnBallAndResetCountdown();
            return;
        }

        bool returnByVelocity = ballRigidbody.velocity.magnitude < ballReturnVelocityThreshold;
        bool returnFromHoleByVelocity = ballRigidbody.velocity.magnitude < returnVelocityThresholdHoleZone;

        if (returnCountdownStarted)
        {
            // If state was changed then ball is probably transitioning from onr zone to another
            // In this case we abort pending countdown
            if (prevState != state)
            {
                returnCountdownStarted = false;
            }

            // Abort transition if ball speed is above current threshold again
            if ((state == BallState.RollingToHole && !returnFromHoleByVelocity) || (state == BallState.InReturnZone && !returnByVelocity))
            {
                returnCountdownStarted = false;
            }

            // If countdown is still running
            if (returnCountdownStarted)
            {
                returnCountdown -= Time.deltaTime;
                if (returnCountdown < 0.0f)
                {
                    // Return ball if countdown has ended
                    ReturnBallAndResetCountdown();
                }
            }
        }
        else
        {
            // Start countdown if the ball's velocity is less than current zone threshold
            if (state == BallState.RollingToHole && returnFromHoleByVelocity)
            {
                returnCountdownStarted = true;
                returnCountdown = returnTimeHoleZone;
            }

            if (state == BallState.InReturnZone && returnByVelocity)
            {
                returnCountdownStarted = true;
                returnCountdown = returnTime;
            }
        }

        // Store the state in order to track transitions from one zone to another
        prevState = state;
    }

    private void OnTriggerEnter(Collider ballHitCollider)
    {
        // If ball hit the hole
        if (ballHitCollider.gameObject.layer == holeLayerIndex)
        {
            // Check, which one was hit using holes tags
            switch (ballHitCollider.gameObject.tag)
            {
                case "AnswerHole1":
                    Debug.Log("Hit #1");
                    break;

                case "AnswerHole2":
                    Debug.Log("Hit #2");
                    break;

                case "AnswerHole3":
                    Debug.Log("Hit #3");
                    break;
            }

            ReturnBallAndResetCountdown();
        }

        if (ballHitCollider.CompareTag("AnswerHole"))
        {
            ballRollingToHole = true;
            OnBallHoleZoneEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider holeFloorCollider)
    {
        if (holeFloorCollider.CompareTag("AnswerHole"))
        {
            ballRollingToHole = false;
            OnBallHoleZoneExit?.Invoke();
        }
    }
}