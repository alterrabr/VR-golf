using UnityEngine;

/// <summary>
/// Interface for teleporation controller. Used for teleport player to a particular position
/// </summary>
public interface ITeleportController
{
    /// <summary>
    /// Teleport player to a given position
    /// <param name="position">Position to teleport</param>
    /// <param name="facingDir">Direction to orient the player after teleportation</param>
    /// </summary>
    void Teleport(Vector3 position, Vector3? facingDir = null);
}