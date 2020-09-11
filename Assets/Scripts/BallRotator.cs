using UnityEngine;
using VRTK;

/// <summary>
/// Rotate grabbed ball based on joystick direction
/// </summary>
public class BallRotator : MonoBehaviour
{
    private VRTK_InteractableObject interactableObject;
    private bool ballIsGrabbed;
    private VRTK_ControllerEvents grabbingObjectControllerEvents;
    private Transform grabbedObjectTransform;
    private Transform grabbingObjectTransform;

    [Header("Usable settings")] 
    [SerializeField]
    private float rotationSpeed = 100f;

    private void Start()
    {
        ballIsGrabbed = false;
        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectGrabbed += BallGrabbed;
        interactableObject.InteractableObjectUngrabbed += BallUngrabbed;
    }

    private void Update()
    {
        RotateObjectInHand();
    }

    private void BallGrabbed(object o, InteractableObjectEventArgs e)
    {
        ballIsGrabbed = true;
        grabbedObjectTransform = transform;
        grabbingObjectControllerEvents = e.interactingObject.GetComponent<VRTK_ControllerEvents>();
        grabbingObjectTransform = e.interactingObject.transform;
    }

    private void BallUngrabbed(object o, InteractableObjectEventArgs e)
    {
        ballIsGrabbed = false;
        grabbingObjectControllerEvents = null;
        grabbingObjectTransform = null;
    }

    private void RotateObjectInHand()
    {
        if (ballIsGrabbed)
        {
            grabbedObjectTransform.rotation =
                Quaternion.AngleAxis(
                    -grabbingObjectControllerEvents.GetTouchpadAxis().x * rotationSpeed * Time.deltaTime,
                    grabbingObjectTransform.forward) * grabbedObjectTransform.rotation;
            grabbedObjectTransform.rotation =
                Quaternion.AngleAxis(
                    grabbingObjectControllerEvents.GetTouchpadAxis().y * rotationSpeed * Time.deltaTime,
                    grabbingObjectTransform.right) * grabbedObjectTransform.rotation;
        }
    }

    private void OnDestroy()
    {
        interactableObject.InteractableObjectGrabbed -= BallGrabbed;
        interactableObject.InteractableObjectUngrabbed -= BallUngrabbed;
    }
}