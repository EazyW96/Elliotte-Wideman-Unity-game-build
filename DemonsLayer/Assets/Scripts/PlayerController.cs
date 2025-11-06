using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerController script is attached to our player.
/// Contains information about the player GameObject
/// Also gets input from the controller and applies the movement to the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Public variables - can be accessed by other scripts and modified in the
    // inspector window in the Unity editor
    public float playerSpeed;           // Movement Speed of the player
    public GameObject laserBlastPrefab; // Prefab GameObject of the laser blasts our player will shoot
    public Transform firePoint;         // Location our player will fire laser blasts from
    public float timeBetweenShots;      // Time in seconds from when the player shoots until he can shoot again

    // Private variables - can be read and modified by this script ONLY
    private Rigidbody2D rb;             // Reference to the Rigidbody2D component of this GameObject
    private Vector2 leftStickInput;     // Current (X,Y) input of the left stick on the game controller
    private Vector2 rightStickInput;    // Current (X,Y) input of the right stick on the game controller
    private bool canShoot;              // True when the player is able to fire a shot

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Get the Rigidbody2D componenet attached to this GameObject
        rb = GetComponent<Rigidbody2D>();

        // Our player is able to shoot from the beginning of the game
        canShoot = true;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Check if the game is in a playing state
        // Will return false during the countdown phase and when all Demons have been defeated
        if (GameController.instance.gamePlaying)
        {
            // We get player input every frame to ensure we have the most up to date input information
            GetPlayerInput();
        }
        else
        {
            // If the game is not playing, we set the stick values to (0,0) so the player doesn't drift/rotate on its own
            leftStickInput = Vector2.zero;
            rightStickInput = Vector2.zero;
        }
        
    }

    /// <summary>
    /// This function gets the most current input from the game controller
    /// </summary>
    private void GetPlayerInput()
    {
        // Gets input of the left stick on the controller
        // Uses the preconfigured "Horizontal" and "Vertical" axes in the Unity Input Project Settings
        leftStickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Gets input of the right stick on the controller
        // Uses the custom "R_Horizontal" and "R_Vertical" axes in the Unity Input Project Settings
        rightStickInput = new Vector2(Input.GetAxis("R_Horizontal"), Input.GetAxis("R_Vertical"));

        // Fire a shot if the "Shoot" button (button we setup in the Unity Input Project Settings)
        // is held down AND the player can shoot - meaning the shot cooldown has elapsed
        if (Input.GetButton("Shoot") && canShoot)
        {
            Shoot();
        }
    }

    /// <summary>
    /// Function instantiates a laser blast at the fire point
    /// </summary>
    private void Shoot()
    {
        // canShoot is set to false so we can put a delay between the shots the player fires
        canShoot = false;

        // spawn a laser blast at the fire point oriented the direction the player is facing
        Instantiate(laserBlastPrefab, firePoint.position, transform.rotation);

        // starts a timer for when the player can shoot again
        StartCoroutine(ShotCooldown());
    }

    /// <summary>
    /// Timer for the when the player can shoot again
    /// </summary>
    /// <returns> Returns once the time between shots has elapsed </returns>
    IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    /// <summary>
    /// Called on a fixed time interval. We use this to move the player independent of framerate
    /// </summary>
    private void FixedUpdate()
    {
        // How much to move the player in the (X,Y) direction this fixed update step.
        // Calculated from the direction of input on the left stick multiplied by
        // the movement speed of the player and normalized for the time elapsed between
        // the previous fixed update call.
        Vector2 curMovement = leftStickInput * playerSpeed * Time.deltaTime;

        // Moves the player to its new position which is calculated by adding the
        // movement of the current fixed update step to its current position
        rb.MovePosition(rb.position + curMovement);

        // Only set the rotation if the right stick if there is input on the right stick
        if(rightStickInput.magnitude > 0f)
        {
            // Helper variable to help us calculate the rotation of the player
            Vector3 curRotation = Vector3.left * rightStickInput.x + Vector3.up * rightStickInput.y;

            // Actual rotation value of the player calculated with the LookRotation() funciton.
            // Vector3.forward is passed in to the function as it is the axis we will be rotating
            // the player around. The directon of V3.forward can be though of as the axis going
            // from the camera, through the top of the player and through the ground
            Quaternion playerRotation = Quaternion.LookRotation(curRotation, Vector3.forward);

            // Apply the calculated rotation to the player's Rigidbody2D
            rb.SetRotation(playerRotation);
        }
    }
}
