using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to the Explosion prefab
/// </summary>
[RequireComponent(typeof(Animator))]    // Ensures there is an Animator component on this GameObject
public class ExplosionController : MonoBehaviour
{
    private Animator myAnimator;        // Animator component of this GameObject

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        // Gets the Animator component attached to this GameObject
        myAnimator = GetComponent<Animator>();

        // Begins the timer that will destroy the explosion once the animation is complete
        StartCoroutine(DestroyExplosion());
    }

    /// <summary>
    /// Destroys the explosion once the animation is complete
    /// </summary>
    IEnumerator DestroyExplosion()
    {
        // Gets information about the current animation state - the explosion animation in this case
        AnimatorStateInfo explosionState = myAnimator.GetCurrentAnimatorStateInfo(0);

        // Returns when the explosion animation is complete
        yield return new WaitForSeconds(explosionState.length);

        // Destroy this GameObject once the animation is complete
        Destroy(gameObject);
    }
}
