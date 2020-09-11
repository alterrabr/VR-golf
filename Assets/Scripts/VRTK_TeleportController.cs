using UnityEngine;
using VRTK;

public class VRTK_TeleportController : MonoBehaviour, ITeleportController
{
    [SerializeField]
    [Tooltip("Reference to a VRTK teleport component ")]
    private VRTK_BasicTeleport vrtkTeleport;

    /// <summary>
    /// Implementation of ITeleportController's method.
    /// It will only rotate player along y axis.
    /// </summary>
    public void Teleport(Vector3 position, Vector3? facingDir = null)
    {
        // Align play area orientation with headset orientation
        // This allow us to implement player rotation by just
        // rotating play area
        var headsetRot = VRTK_DeviceFinder.HeadsetCamera().localRotation;
        var headsetRotInverse = Quaternion.Inverse(headsetRot);
        var euler = headsetRotInverse.eulerAngles;
        // Discard x and z axis rotations. We only rotate along y axis
        var yHeadsetRot = Quaternion.Euler(0.0f, euler.y, 0.0f);

        // Discard x and z axis. For now we only rotate along y axis
        var facingDirY = new Vector3(0.0f, facingDir.Value.y, 0.0f);
        // Just rotate play area now
        var finalRot = yHeadsetRot * Quaternion.LookRotation(facingDir.Value);
        // ForceTeleport will rotate play area
        vrtkTeleport.ForceTeleport(position, finalRot);
    }
}
