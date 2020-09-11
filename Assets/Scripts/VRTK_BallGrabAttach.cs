using UnityEngine;
using VRTK.GrabAttachMechanics;

/// <summary>
/// Sets the grabbed Ball with collider turned off.
/// </summary>
public class VRTK_BallGrabAttach : VRTK_ChildOfControllerGrabAttach
{
    private Collider grabbedObjectCollider;

    /// <summary>
    /// The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed. It is also responsible for creating the joint on the grabbed object.
    /// </summary>
    /// <param name="grabbingObject">The GameObject that is doing the grabbing.</param>
    /// <param name="givenGrabbedObject">The GameObject that is being grabbed.</param>
    /// <param name="givenControllerAttachPoint">The point on the grabbing object that the grabbed object should be attached to after grab occurs.</param>
    /// <returns>Returns `true` if the grab is successful, `false` if the grab is unsuccessful.</returns>
    public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject,
        Rigidbody givenControllerAttachPoint)
    {
        if (base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint))
        {
            SnapObjectToGrabToController(givenGrabbedObject);
            grabbedObjectScript.isKinematic = true;
            grabbedObjectCollider = grabbedObjectScript.GetComponent<Collider>();
            grabbedObjectCollider.enabled = false;
            return true;
        }

        return false;
    }

    /// <summary>
    /// The StopGrab method ends the grab of the current Interactable Object and cleans up the state.
    /// </summary>
    /// <param name="applyGrabbingObjectVelocity">If `true` will apply the current velocity of the grabbing object to the grabbed object on release.</param>
    public override void StopGrab(bool applyGrabbingObjectVelocity)
    {
        grabbedObjectCollider.enabled = true;
        base.StopGrab(applyGrabbingObjectVelocity);
    }
}