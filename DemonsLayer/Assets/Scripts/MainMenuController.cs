using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;               // Included to modify Unity UI components
using UnityEngine.SceneManagement;  // Included to load into different scenes

/// <summary>
/// Attached to the Main Menu Controller GameObject in the Main Menu scene
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // Public GameObjects set in the Inspector in Unity
    public GameObject mainScreen, aboutScreen;

    // Public Buttons set in the Inspector in Unity
    public Button playButton, backButton;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        // Selects the play button for use with a game controller
        playButton.Select();
    }

    /// <summary>
    /// Called when the Play Game Button is pressed on the Main Menu screen.
    /// Set in the OnClick() parameter of the button component in the Unity Inspector
    /// </summary>
    public void OnButtonPlayGame()
    {
        // Loads the main game scene
        SceneManager.LoadScene("TheLair");
    }

    /// <summary>
    /// Called when the About Button is pressed on the Main Menu screen.
    /// Set in the OnClick() parameter of the button component in the Unity Inspector
    /// </summary>
    public void OnButtonAboutGame()
    {
        // Disables the main screen
        mainScreen.SetActive(false);

        // Enables the about screen
        aboutScreen.SetActive(true);

        // Selects the back button for use with a game controller
        backButton.Select();
    }

    /// <summary>
    /// Called when the Back Button is pressed on the About screen.
    /// Set in the OnClick() parameter of the button component in the Unity Inspector
    /// </summary>
    public void OnButtonBack()
    {
        // Enables the main screen
        mainScreen.SetActive(true);

        // Disables the about screen
        aboutScreen.SetActive(false);

        // Selects the play button for use with a game controller
        playButton.Select();
    }

    /// <summary>
    /// Called when the Play Game Button is pressed on the Main Menu screen.
    /// Set in the OnClick() parameter of the button component in the Unity Inspector
    /// </summary>
    public void OnButtonQuit()
    {
        // Closes the game
        Application.Quit();
    }
}
