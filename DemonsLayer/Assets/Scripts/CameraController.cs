using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to the MainCamera GameObject
/// Follows the player specified in the public Transform
/// </summary>
public class CameraController : MonoBehaviour
{
    // Reference to the player's transform component that the camera will be following
    public Transform playerTransform;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // Sets the position of the camera to match the X and Y position of the player
        // Keeps its current Z coordinate to ensure the camera is above the player/game world
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    }
}
