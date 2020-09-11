using UnityEngine;

/// <summary>
/// Follower of club end for correct physics
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ClubPunchBoxFollower : MonoBehaviour
{
    private Transform clubPunchBoxTransform;

    [SerializeField]
    [Tooltip("Force which break fixed joint")]
    private float forceForBreakJoint = 5f;
    
    [SerializeField]
    [Tooltip("(For joint) The scale to apply to the inverse mass and inertia tensor of the body prior to solving the constraints")]
    private float massScale = 50f;
    
    [SerializeField]
    [Tooltip("(For joint) The scale to apply to the inverse mass and inertia tensor of the connected body prior to solving the constraints")]
    private float connectedMassScale = 50f;

    /// <summary>
    /// Set follower position and rotation to ClubPunchBox
    /// </summary>
    public void SetFollowTarget(Transform punchBoxTransform)
    {
        clubPunchBoxTransform = punchBoxTransform;
    }

    private void Start()
    {
        SetupFixedJoint();
    }

    private void ReturnFollowerToClubPunchBox()
    {
        transform.position = clubPunchBoxTransform.position;
        transform.rotation = clubPunchBoxTransform.rotation;
    }

    private void SetupFixedJoint()
    {
        ReturnFollowerToClubPunchBox();
        FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = clubPunchBoxTransform.GetComponentInParent<Rigidbody>();
        fixedJoint.breakForce = forceForBreakJoint;
        fixedJoint.massScale = massScale;
        fixedJoint.connectedMassScale = connectedMassScale;
    }
}