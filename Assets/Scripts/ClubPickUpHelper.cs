using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRTK;

/// <summary>
/// Change colliders when pick up the club and create follower
/// </summary>
public class ClubPickUpHelper : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Object, which placed on the end of club and need for correct physics punch work")]
    private GameObject clubPunchBox;

    [SerializeField]
    [Tooltip("List of colliders, attached to club")]
    private List<BoxCollider> clubColliders;

    [SerializeField]
    private ClubPunchBoxFollower clubFollowerPrefab;

    private VRTK_InteractableObject interactableObject;

    private ClubPunchBoxFollower follower;

    private void Start()
    {
        interactableObject = GetComponent<VRTK_InteractableObject>();

        interactableObject.InteractableObjectGrabbed += ClubGrabbed;
        interactableObject.InteractableObjectUngrabbed += ClubUngrabbed;
    }

    /// <summary>
    /// Create follower object to perform punch.
    /// </summary>
    private void SpawnClubFollower()
    {
        if (follower == null)
        {
            follower = Instantiate(
                clubFollowerPrefab,
                clubPunchBox.transform.position,
                clubPunchBox.transform.rotation
            );

            follower.SetFollowTarget(clubPunchBox.transform);
        } 
        else
        {
            follower.SetFollowTarget(clubPunchBox.transform);
        }
    }

    private void ClubGrabbed(object o, InteractableObjectEventArgs e)
    {
        SpawnClubFollower();

        if(interactableObject.IsGrabbed())
        {
            ToggleCollidersTriggersState(true);
        }  
    }

    private void ClubUngrabbed(object o, InteractableObjectEventArgs e)
    {
        if (follower != null && !interactableObject.IsGrabbed())
        {
            Destroy(follower.gameObject);
            ToggleCollidersTriggersState(false);
        }
    }

    private void OnDestroy()
    {
        //interactableObject.InteractableObjectGrabbed -= ClubGrabbed;
        //interactableObject.InteractableObjectUngrabbed -= ClubUngrabbed;
    }

    // Toggle IsTrigger property in colliders, applied to club.
    // If club is grabbed, colliders must be just trigger.
    // If club is not grabbed, colliders must apply physics to club.
    private void ToggleCollidersTriggersState(bool state)
    {
        clubColliders.All(collider => { collider.isTrigger = state; return true; });
    }
}