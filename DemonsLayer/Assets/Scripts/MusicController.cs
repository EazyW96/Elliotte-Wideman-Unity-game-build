using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton attached to the Music Controller GameObject.
/// Each scene has a Music Controller GameObject so no matter which scene you begin the game in,
/// music will always be playing. This script checks for other instances of the Music Controller
/// and it will destroy them. End result is that the Music Controller will persist throughout
/// the game and loading levels without interrupting the background music playback
/// </summary>
public class MusicController : MonoBehaviour
{
    private static MusicController _instance;
    public static MusicController instance;

    /// <summary>
    /// Called before the start function when the scene is loaded
    /// </summary>
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
        instance = _instance;

        // Ensures that this object will persist throughout the loading of scenes
        DontDestroyOnLoad(this);
    }
}
