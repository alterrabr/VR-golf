using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class is responsible for managinf controllers mutual blocking,
/// i.e. when try teleport using left controller, right will be blocked
/// </summary>
public class ControllerManager
{
    private struct ControllerState
    {
        public bool teleportButtonPressed;
        public int lastChangeFrame;
    }

    private GameStateManager gameStateManager;

    private ControllerState[] controllerStates = new ControllerState[2];
    private HandController[] controllers = new HandController[2];

    private HandController leftController;
    private HandController rightController;

    /// <summary>
    /// Constructor of a ControllerManager class
    /// </summary>
    public ControllerManager(GameStateManager manager, HandController right, HandController left)
    {
        leftController = left;
        rightController = right;

        controllers[0] = right;
        controllers[1] = left;

        gameStateManager = manager;

        rightController.OnTeleportButtonStateChange += OnControllerTeleportButtonStateChange;
        leftController.OnTeleportButtonStateChange += OnControllerTeleportButtonStateChange;
    }

    /// <summary>
    /// Unsubscribe all events
    /// </summary>
    public void UnsubscribeEvents()
    {
        rightController.OnTeleportButtonStateChange -= OnControllerTeleportButtonStateChange;
        leftController.OnTeleportButtonStateChange -= OnControllerTeleportButtonStateChange;
    }

    private void OnControllerTeleportButtonStateChange(HandController controller, bool state)
    {
        // Update controllers state
        if (controller == rightController)
        {
            controllerStates[0].teleportButtonPressed = state;
            controllerStates[0].lastChangeFrame = Time.frameCount;
        }
        else
        {
            controllerStates[1].teleportButtonPressed = state;
            controllerStates[1].lastChangeFrame = Time.frameCount;
        }

        // Execute this as coroutine. We need to wait one frame. Otherwise
        // controllers might be blocked before they finish previous teleport actions
        gameStateManager.StartCoroutine(ResolveControllerPermissionsCoroutine());
    }

    private IEnumerator ResolveControllerPermissionsCoroutine()
    {
        yield return null;

        // Find a controller which acquire teleport first
        int min = Int32.MaxValue;
        int index = -1;

        for (int i = 0; i < controllerStates.Length; i++)
        {
            var controller = controllerStates[i];

            if (controller.teleportButtonPressed && controller.lastChangeFrame < min && !controllers[i].Grabbing)
            {
                min = controller.lastChangeFrame;
                index = i;
            }
        }

        // If there is no controllers that currently use teleport then release all blocks
        if (index == -1)
        {
            foreach (var controller in controllers)
            {
                controller.BlockTeleport(false);
            }
        }
        else
        {
            // Otherwise block all controllers except one that acquired teleport first
            for (int i = 0; i < controllers.Length; i++)
            {
                var controller = controllers[i];
                controller.BlockTeleport(i != index);
            }
        }
    }
}
