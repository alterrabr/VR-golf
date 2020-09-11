using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Implementation of HandController abstract class for left hand controller
/// </summary>
public class RightHandController : HandController
{
    private bool teleportMode;
    private bool teleportBlocked;

    public RightHandController(VRTK_HandController handController) : base(handController)
    {
        controller.OnTeleportButtonStateChange += (state) => SetTeleportStateInGameMode(state); //reaction to pressing the touch panel to switch the
    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        controller.OnTeleportButtonStateChange -= (state) => SetTeleportStateInGameMode(state);
    }

    protected override void SetControllerModeMenu()
    {
        controller.EnableInteractions(false);
        controller.EnableTeleport(false);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Straight, VRTK_HandController.BeamMode.AlwaysShow);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, VRTK_HandController.BeamMode.AlwaysHide);
        controller.EnableUIInteraction(true);
    }

    protected override void SetControllerModeGame()
    {
        controller.EnableInteractions(true);
        controller.EnableTeleport(true);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Straight, VRTK_HandController.BeamMode.AlwaysShow);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, VRTK_HandController.BeamMode.AlwaysShow);
        controller.EnableUIInteraction(true);
    }

    protected override void SetControllerModeGrab()
    {
        controller.EnableInteractions(true);
        controller.EnableTeleport(false);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Straight, VRTK_HandController.BeamMode.AlwaysHide);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, VRTK_HandController.BeamMode.AlwaysHide);
        controller.EnableUIInteraction(false);
    }

    /// <summary>
    /// In game mode, the right controller uses a straight pointer if the touchpad is not
    /// pressed, otherwise this controller uses a Bezier
    /// </summary>
    /// <param name="activeState"></param>
    private void SetTeleportStateInGameMode(bool activeState)
    {
        if (controllerMode == Mode.Game)
        {
            teleportMode = activeState;
            if(activeState && !teleportBlocked)
            {
                controller.UseBezier();
            }
            else
            {
                controller.UseStraight();
            }
        }
    }

    public override void BlockTeleport(bool block)
    {
        if(controllerMode == Mode.Game)
        {
            teleportBlocked = block;
            controller.EnableInteractions(true);
            controller.EnableTeleport(!block);
            controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, block ? VRTK_HandController.BeamMode.AlwaysHide : VRTK_HandController.BeamMode.AlwaysShow);
            controller.EnableUIInteraction(true);

            if(!block && teleportMode)
            {
                controller.UseBezier();
            }
            else
            {
                controller.UseStraight();
            }
        }
    }
}
