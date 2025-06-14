using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls menu navigation and game state (pause, resume, restart, quit).
/// </summary>
public class MenuController : MonoBehaviour
{
    [Tooltip("Pause menu GameObject")]
    public GameObject PauseMenu;

    [Tooltip("Credits menu GameObject")]
    public GameObject CreditMenu;

    [Tooltip("Main menu GameObject")]
    public GameObject MainMenu;

    /// <summary>
    /// Pauses the game and shows the pause menu.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
    }

    /// <summary>
    /// Resumes the game and hides the pause menu.
    /// </summary>
    public void Continue()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
    }

    /// <summary>
    /// Restarts the current game scene.
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void MainMenuLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Starts the game by loading the game scene.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Shows the credits menu and hides the main menu.
    /// </summary>
    public void Credit()
    {
        CreditMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    /// <summary>
    /// Hides the credits menu and shows the main menu.
    /// </summary>
    public void CreditReturn()
    {
        CreditMenu.SetActive(false);
        MainMenu.SetActive(true);
    }
}
