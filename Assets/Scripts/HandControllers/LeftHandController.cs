using UnityEngine;

/// <summary>
/// Implementation of HandController abstract class for left hand controller
/// </summary>
public class LeftHandController : HandController
{
    public LeftHandController(VRTK_HandController handController) : base(handController)
    {
    }

    protected override void SetControllerModeMenu()
    {
        controller.EnableInteractions(false);
        controller.EnableTeleport(false);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Straight, VRTK_HandController.BeamMode.AlwaysHide);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, VRTK_HandController.BeamMode.AlwaysHide);
        controller.EnableUIInteraction(false);
    }

    protected override void SetControllerModeGame()
    {
        controller.EnableInteractions(true);
        controller.EnableTeleport(true);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Straight, VRTK_HandController.BeamMode.AlwaysHide);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, VRTK_HandController.BeamMode.ShowOnPress);
        controller.EnableUIInteraction(false);
    }

    protected override void SetControllerModeGrab()
    {
        controller.EnableInteractions(true);
        controller.EnableTeleport(false);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Straight, VRTK_HandController.BeamMode.AlwaysHide);
        controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, VRTK_HandController.BeamMode.AlwaysHide);
        controller.EnableUIInteraction(false);
    }

    public override void BlockTeleport(bool block)
    {
        if (controllerMode == Mode.Game)
        {
            controller.EnableInteractions(true);
            controller.EnableTeleport(!block);
            controller.SetBeamMode(VRTK_HandController.PointerMode.Bezier, block ? VRTK_HandController.BeamMode.AlwaysHide : VRTK_HandController.BeamMode.ShowOnPress);
            controller.EnableUIInteraction(false);
        }
    }
}
