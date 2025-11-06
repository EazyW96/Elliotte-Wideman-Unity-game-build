using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script is attached to the Demon prefab
/// Keeps track of the number of hit points this instance has
/// and controls associated effects like animation and sound effects
/// </summary>
[RequireComponent(typeof(Animator))]    // Ensures there is an Animator component on this GameObject
[RequireComponent(typeof(AudioSource))] // Ensures there is an AudioSource component on this GameObject
public class DemonController : MonoBehaviour
{
    public int hitPoints;           // Number of times the demon needs to be hit to defeat

    private Animator myAnimator;    // Animator component of this GameObject
    private AudioSource hitSound;   // AudioSource that plays hit/death sounds (set to hit sound in the inspector)
    private AudioClip deathSound;   // AudioClip to play when the Demon is defeated

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        // Gets the Animator component attached to this GameObject
        myAnimator = GetComponent<Animator>();

        // Gets the AudioSource attached to this GameObject
        hitSound = GetComponent<AudioSource>();

        // Loads the DemonWail sound effect from the "Resources" folder
        deathSound = Resources.Load<AudioClip>("Audio/DemonWail");
    }

    /// <summary>
    /// Called when this GameObject collides with another GameObject with a Collider2D component
    /// This will only be called when it collides with objects on physics layers that we
    /// specified in the Unity Physics 2D Project Settings.
    /// </summary>
    /// <param name="collision"> Contains information about the collision between the two objects </param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the GameObject we are colliding with is tagged as a "Bullet" ...
        if(collision.gameObject.tag == "Bullet")
        {
            // ... decrement the hit points by 1 ...
            hitPoints--;

            // ... then check if the demon is out of hit points ...
            if(hitPoints <= 0)
            {
                // Set the clip of the AudioSource to the death sound
                hitSound.clip = deathSound;

                // Start the timer to destroy the Demon GameObject after the death animation completes
                StartCoroutine(KillDemon());
            }

            // Plays the current hit sound. If the Demon is out of hit points,
            // this will be the death sound. Otherwise this will be the default
            // hit sound set in the Unity inspector
            hitSound.Play();
        }
    }

    /// <summary>
    /// Sets some parameters when the Demon is out of hit points
    /// Once the death animation is complete, the Demon Gameobject is destroyed
    /// </summary>
    /// <returns> Returns when death animation is complete </returns>
    IEnumerator KillDemon()
    {
        // Disables the Demon's collider so objects no longer collide with it when the death animation is playing
        GetComponent<Collider2D>().enabled = false;

        // Calls the SlayDemon function on the GameController singleton
        GameController.instance.SlayDemon();

        // Triggers the "Kill Demon" animation trigger, which we setup to play the Demon death animation
        myAnimator.SetTrigger("Kill Demon");

        // Gets information about the current animation state - the death animation in this case
        AnimatorStateInfo deathAnimState = myAnimator.GetCurrentAnimatorStateInfo(0);

        // Returns when the death animation is complete
        yield return new WaitForSeconds(deathAnimState.length);

        // Destroys the Demon GameObject
        Destroy(gameObject);
    }
}
