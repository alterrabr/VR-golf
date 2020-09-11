using UnityEngine;
using System;

/// <summary>
/// Abstract class for managing controller state
/// </summary>
public abstract class HandController
{
    /// <summary>
    /// Controller mode which determines it's internal state
    /// </summary>
    public enum Mode
    {
        Game,
        Menu,
        Grab
    }

    /// <summary>
    /// True if controller currently grabbing item
    /// </summary>
    public bool Grabbing
    {
        get
        {
            return grabbing;
        }
    }

    /// <summary>
    /// This asction will be called when controller grabs something
    /// </summary>
    public event Action OnGrab;

    /// <summary>
    /// This asction will be called when controller ungrabs something
    /// </summary>
    public event Action OnUngrab;

    /// <summary>
    /// This asction will be called when controller teleport button state changed
    /// </summary>
    public event Action<HandController, bool> OnTeleportButtonStateChange;

    protected VRTK_HandController controller;

    protected Mode controllerMode = Mode.Menu;

    private bool grabbing;

    /// <summary>
    /// Contructor
    /// <param name="handController">Reference to a VRRK_HandController which will bw managed by this instance</param>
    /// </summary>
    public HandController(VRTK_HandController handController)
    {
        controller = handController;
        SetControllerMode(controllerMode);

        handController.OnGrab += OnControllerGrab;
        handController.OnUngrab += OnControllerUngrab;

        handController.OnTeleportButtonStateChange += OnTeleportButtonInvoke;

        handController.OnSDKManagerLoadedSetupChanged += OnSDKManagerLoadedSetupChanged;
    }

    /// <summary>
    /// Unsubscribe all internal events
    /// </summary>
    public virtual void UnsubscribeEvents()
    {
        controller.OnGrab -= OnControllerGrab;
        controller.OnUngrab -= OnControllerUngrab;

        controller.OnTeleportButtonStateChange -= OnTeleportButtonInvoke;

        controller.OnSDKManagerLoadedSetupChanged -= OnSDKManagerLoadedSetupChanged;
    }

    /// <summary>
    /// Change mode of the controller
    /// </summary>
    public void SetControllerMode(Mode mode)
    {
        if (controller != null)
        {
            switch (mode)
            {
                case Mode.Menu:
                {
                    controllerMode = Mode.Menu;
                    SetControllerModeMenu();
                } break;

                case Mode.Game:
                {
                    controllerMode = Mode.Game;
                    SetControllerModeGame();
                } break;

                case Mode.Grab:
                {
                    controllerMode = Mode.Grab;
                    SetControllerModeGrab();
                }
                break;

                default:
                {
                    Debug.LogError("Invalid default case");
                } break;
            }
        }
    }

    /// <summary>
    /// Ungrab currently grabbed item. Will also trigger OnUngrab event
    /// </summary>
    public void Ungrab()
    {
        controller.Ungrab();
        grabbing = false;
    }

    /// <summary>
    /// Enable/disable teleport for this controller
    /// </summary>
    /// <param name="block">If value is true teleport will be work as usually, else - blocked</param>
    public abstract void BlockTeleport(bool block);

    /// <summary>
    /// Implementation of this method should configure state of the controller for the Menu mode
    /// </summary>
    protected abstract void SetControllerModeMenu();

    /// <summary>
    /// Implementation of this method should configure state of the controller for the Game mode
    /// </summary>
    protected abstract void SetControllerModeGame();

    /// <summary>
    /// Implementation of this method should configure state of the controller for the Game mode
    /// </summary>
    protected abstract void SetControllerModeGrab();

    private void OnTeleportButtonInvoke(bool state)
    {
        OnTeleportButtonStateChange?.Invoke(this, state);
    }

    private void OnSDKManagerLoadedSetupChanged()
    {
        SetControllerMode(controllerMode);
    }

    private void OnControllerGrab()
    {
        grabbing = true;
        OnGrab?.Invoke();
    }

    private void OnControllerUngrab()
    {
        grabbing = false;
        OnUngrab?.Invoke();
    }
}
