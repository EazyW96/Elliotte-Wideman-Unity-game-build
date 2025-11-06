using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;               // Included to modify Unity UI components
using UnityEngine.SceneManagement;  // Included to load into different scenes

/// <summary>
/// GameController class is attached to one GameObject only
/// This keeps track of all the variables in the game such as number of demons killed
/// and the amount of time the player has been playing for. Also contains code for
/// displaying UI on the HUD.
/// </summary>
public class GameController : MonoBehaviour
{
    // Simple implementation of a Singleton. See MusicController.cs for a better
    // way to implement a Singleton class.
    public static GameController instance;

    // Public GameObjects we set in the Unity Inspector
    public GameObject demonContainer, hudContainer, gameOverPanel;

    // Public Text components we set in the Unity Inspector
    public Text demonCounter, timeCounter, countdownText;

    // True if the game is playing, false if it is not
    // Returns false during countdown at beginning and after the last Demon is destroyed
    // Other classes can read this variable, but it can only be set within this class
    public bool gamePlaying { get; private set; }

    // Number of seconds to count down before starting the game
    public int countdownTime;

    // Number of Demons in the scene and number of demons we've destroyed
    private int numTotalDemons, numSlayedDemons;

    // Time in seconds of when the game began and how long the game has been playing
    private float startTime, elapsedTime;

    // Amount of time the game has been playing. Can be formatted nicely
    TimeSpan timePlaying;

    /// <summary>
    /// Called before the start function when the scene is loaded
    /// </summary>
    private void Awake()
    {
        // Assigns this object to the public static instance (singleton)
        // See the MusicController.cs for a better implementation of a singleton
        instance = this;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        // Gets the number of total demons in the world based off the number of children in the demonContainer GameObject
        numTotalDemons = demonContainer.transform.childCount;

        // At the beginning of the level, the player has defeated 0 demons
        numSlayedDemons = 0;

        // Initialize the Demon counter UI
        demonCounter.text = "Demons: 0 / " + numTotalDemons;

        // Initialize the time counter UI
        timeCounter.text = "Time: 00:00.00";

        // Game is set to false until countdown completes
        gamePlaying = false;

        // Begins the countdown at the start of the game
        StartCoroutine(CountdownToStart());
    }

    /// <summary>
    /// Sets some variables at the start of the game
    /// </summary>
    private void BeginGame()
    {
        // Game is now playing and player can move around
        gamePlaying = true;

        // Sets the time the game began playing
        startTime = Time.time;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // If the game is in a playing state ...
        if (gamePlaying)
        {
            // ... Calculate the time elapsed since the start of the game
            elapsedTime = Time.time - startTime;

            // Generate a TimeSpan from the number of seconds the game has been going on for
            timePlaying = TimeSpan.FromSeconds(elapsedTime);

            // Nicely formatted string of the time the game has been playing
            // Example format - Time: 01:23.45
            string timePlayingStr = "Time: " + timePlaying.ToString("mm':'ss'.'ff");

            // Sets the time counter UI to our nicely formatted string
            timeCounter.text = timePlayingStr;
        }
    }

    /// <summary>
    /// Called from the DemonController.cs when a Demon is defeated
    /// </summary>
    public void SlayDemon()
    {
        // Increment the number of Demons slayed by 1
        numSlayedDemons++;

        // Creates a string to display for the number of demons we have defeated out of the total number of Demons
        // Example format - Demons: 4 / 18
        string demonCounterStr = "Demons: " + numSlayedDemons + " / " + numTotalDemons;
        
        // Sets the Demon counter UI to the string we generated above
        demonCounter.text = demonCounterStr;

        // Checks if we have defeated all the demons in the level
        if(numSlayedDemons >= numTotalDemons)
        {
            // Calls the End Game function
            EndGame();
        }
    }

    /// <summary>
    /// Called when all demons have been defeated
    /// </summary>
    private void EndGame()
    {
        // Game is no longer playing so player cannot input anything on the controls
        gamePlaying = false;

        // Call the ShowGameOverScreen() function in 1.25 seconds
        Invoke("ShowGameOverScreen", 1.25f);
    }

    /// <summary>
    /// Displays the Game Over screen at the end of the level
    /// </summary>
    private void ShowGameOverScreen()
    {
        // Enable the Game Over screen
        gameOverPanel.SetActive(true);

        // Disable the HUD
        hudContainer.SetActive(false);

        // Creates a nicely formatted time string for the final time
        string timePlayingStr = "Time: " + timePlaying.ToString("mm':'ss'.'ff");

        // Sets the final time UI component on the Game Over screen
        gameOverPanel.transform.Find("FinalTimeText").GetComponent<Text>().text = timePlayingStr;

        // Selects the try again button for controller input
        gameOverPanel.transform.Find("RestartButton").GetComponent<Button>().Select();
    }

    /// <summary>
    /// Provides a graphical countdown at the beginning of the game
    /// </summary>
    IEnumerator CountdownToStart()
    {
        // While the countdown time is greater than zero...
        while (countdownTime > 0)
        {
            // Set the countdown UI component to the current countdown time integer
            countdownText.text = countdownTime.ToString();

            // Return in exactly 1 second
            yield return new WaitForSeconds(1f);

            // Decrement the countdown time integer by 1
            countdownTime--;
        }

        // Once the countdown timer reaches 0, call the BeginGame() fucntion
        BeginGame();

        // Sets the countdown UI to "GO!"
        countdownText.text = "GO!";

        // Return in exactly 1 second
        yield return new WaitForSeconds(1f);

        // Disable the countdown UI to hid the "GO!" text
        countdownText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when one of the buttons on the Game Over screen is pressed
    /// </summary>
    /// <param name="levelToLoad"> Specify the level that should be loaded </param>
    public void OnButtonLoadLevel(string levelToLoad)
    {
        // Loads the specified scene
        SceneManager.LoadScene(levelToLoad);
    }
}
