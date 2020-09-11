using UnityEngine;
using VRTK;
using System;
using VRTK.UnityEventHelper;
using static VRTK.VRTK_SDKManager;
using System.Collections;

/// <summary>
/// A wrapper class which presents VRTK hand controller capabilities in unified way
/// </summary>
[RequireComponent(typeof(VRTK_UIPointer))]
[RequireComponent(typeof(VRTK_Pointer))]
[RequireComponent(typeof(VRTK_InteractTouch))]
[RequireComponent(typeof(VRTK_InteractGrab))]
[RequireComponent(typeof(VRTK_InteractUse))]
[RequireComponent(typeof(VRTK_InteractNearTouch))]
[RequireComponent(typeof(VRTK_StraightPointerRenderer))]
[RequireComponent(typeof(VRTK_InteractGrab_UnityEvents))]
public class VRTK_HandController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to a VRTK_SDKManager of managed controller")]
    private VRTK_SDKManager manager;

    [SerializeField]
    [Tooltip("Reference to a VRTK_Pointer of managed controller")]
    private VRTK_Pointer pointer;

    [SerializeField]
    [Tooltip("Reference to a VRTK_BezierPointerRenderer of managed controller")]
    private VRTK_BezierPointerRenderer bezierPointerRenderer;

    [SerializeField]
    [Tooltip("Reference to a VRTK_StraightPointerRenderer of managed controller")]
    private VRTK_StraightPointerRenderer straightPointerRenderer;

    [SerializeField]
    [Tooltip("Reference to a VRTK_UIPointer of managed controller")]
    private VRTK_UIPointer uiPointer;

    [SerializeField]
    [Tooltip("Reference to a VRTK_InteractTouch of managed controller")]
    private VRTK_InteractTouch touchInteract;

    [SerializeField]
    [Tooltip("Reference to a VRTK_InteractGrab of managed controller")]
    private VRTK_InteractGrab grabInteract;

    [SerializeField]
    [Tooltip("Reference to a VRTK_InteractGrab_UnityEvents of managed controller")]
    private VRTK_InteractGrab_UnityEvents grabInteractEvents;

    [SerializeField]
    [Tooltip("Reference to a VRTK_InteractUse of managed controller")]
    private VRTK_InteractUse useInteract;

    [SerializeField]
    [Tooltip("Reference to a VRTK_InteractNearTouch of managed controller")]
    private VRTK_InteractNearTouch nearToushInteract;

    [SerializeField]
    [Tooltip("Reference to a VRTK_ControllerEvents of managed controller")]
    private VRTK_ControllerEvents controllerEvents;

    private bool toStraightCoroutineRunning;

    /// <summary>
    /// A list of available modes for controller beam
    /// </summary>
    public enum BeamMode
    {
        AlwaysHide = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff,
        AlwaysShow = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn,
        ShowOnPress = VRTK_BasePointerRenderer.VisibilityStates.OnWhenActive
    };

    public enum PointerMode
    {
        Straight,
        Bezier
    };

    /// <summary>
    /// This action will be called when controller grabs an object
    /// </summary>
    public event Action OnGrab;

    /// <summary>
    /// This action will be called when controller releases grabbed object
    /// </summary>
    public event Action OnUngrab;

    /// <summary>
    /// This action will be called when controller's touchpad will be pressed or released
    /// </summary>
    public event Action<bool> OnTeleportButtonStateChange;

    /// <summary>
    /// This action will be called when controller's touchpad will be released after a pressed state
    /// </summary>
    public event Action OnTouchpadUp;

    /// <summary>
    /// This action will be called when VRTK_SDKManager invokes the LoadedSetupChanged event
    /// </summary>
    public event Action OnSDKManagerLoadedSetupChanged;

    /// <summary>
    /// Enable all interactions for a controller
    /// </summary>
    public void EnableInteractions(bool enable)
    {
        touchInteract.enabled = enable;
        grabInteract.enabled = enable;
        useInteract.enabled = enable;
        // We do not enable VRTK_InteractNearTouch conponent because it makes
        // for some reason pointers to collide with controllers. We do not use this component
        // in the game so just disable it entirely
        //nearToushInteract.enabled = enable;
    }

    /// <summary>
    /// Enable teleport for a controller
    /// </summary>
    public void EnableTeleport(bool enable)
    {
        pointer.enableTeleport = enable;
    }

    /// <summary>
    /// Enable interaction with a UI for a controller
    /// </summary>
    public void EnableUIInteraction(bool enable)
    {
        uiPointer.enabled = enable;
    }

    /// <summary>
    /// Set mode for a controller beam
    /// </summary>
    public void SetBeamMode(PointerMode pointer, BeamMode mode)
    {
        VRTK_BasePointerRenderer.VisibilityStates visibilityState = (VRTK_BasePointerRenderer.VisibilityStates)mode;

        VRTK_BasePointerRenderer renderer = null;

        switch (pointer)
        {
            case PointerMode.Straight: { renderer = straightPointerRenderer; } break;
            case PointerMode.Bezier: { renderer = bezierPointerRenderer; } break;
            default: { Debug.LogError("Invalid default"); } break;
        }

        renderer.cursorVisibility = visibilityState;
        renderer.tracerVisibility = visibilityState;
    }

    /// <summary>
    /// Ungrab currently grabbed item. Will also trigger OnUngrab event
    /// </summary>
    public void Ungrab()
    {
        grabInteract.ForceRelease();
    }

    /// <summary>
    /// This method set up the controller for using BezierPointRenderer
    /// </summary>
    public void UseBezier()
    {
        pointer.pointerRenderer = bezierPointerRenderer;
        bezierPointerRenderer.enabled = true;
        straightPointerRenderer.enabled = false;
    }

    /// <summary>
    /// This method set up the controller for using StraightPointerRenderer
    /// </summary>
    public void UseStraight()
    {
        // Using coroutine to skip one frame. Changing pointer immediately
        // might cause problems when we do teleport at the same frame when changing pointer
        // Teleportation will happen after the pointer will be changed and it will use
        // last straight pointer hit position as teleport destinations
        if (!toStraightCoroutineRunning)
        {
            toStraightCoroutineRunning = true;
            StartCoroutine(UseStraightDeffered());
        }
    }

    private IEnumerator UseStraightDeffered()
    {
        straightPointerRenderer.cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
        straightPointerRenderer.tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
        yield return null;
        pointer.pointerRenderer = straightPointerRenderer;
        straightPointerRenderer.enabled = true;
        bezierPointerRenderer.enabled = false;

        straightPointerRenderer.UpdateRenderer();

        yield return null;

        straightPointerRenderer.cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
        straightPointerRenderer.tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
        toStraightCoroutineRunning = false;
    }

    private void Awake()
    {
        grabInteractEvents.OnControllerGrabInteractableObject.AddListener(OnGrabInteractGrab);
        grabInteractEvents.OnControllerUngrabInteractableObject.AddListener(OnGrabInteractUngrab);
        controllerEvents.TouchpadPressed += OnTouchadClick;
        controllerEvents.TouchpadReleased += OnTouchadRelease;
        manager.LoadedSetupChanged += CheckControllerMode;
    }

    private void OnDestroy()
    {
        grabInteractEvents.OnControllerGrabInteractableObject.RemoveListener(OnGrabInteractGrab);
        grabInteractEvents.OnControllerUngrabInteractableObject.RemoveListener(OnGrabInteractUngrab);
        controllerEvents.TouchpadPressed -= OnTouchadClick;
        controllerEvents.TouchpadReleased -= OnTouchadRelease;
        manager.LoadedSetupChanged -= CheckControllerMode;
    }

    private void OnGrabInteractGrab(object obj, ObjectInteractEventArgs args)
    {
        OnGrab?.Invoke();
    }

    private void OnGrabInteractUngrab(object obj, ObjectInteractEventArgs args)
    {
        OnUngrab?.Invoke();
    }

    private void OnTouchadClick(object obj, ControllerInteractionEventArgs args)
    {
        OnTeleportButtonStateChange?.Invoke(true);
    }

    private void OnTouchadRelease(object obj, ControllerInteractionEventArgs args)
    {
        OnTeleportButtonStateChange?.Invoke(false);
    }

    private void CheckControllerMode(VRTK_SDKManager sender, LoadedSetupChangeEventArgs e)
    {
        OnSDKManagerLoadedSetupChanged?.Invoke();
    }
}