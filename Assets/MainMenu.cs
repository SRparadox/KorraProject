using UnityEngine;
using UnityEngine.SceneManagement; // Need to import Scene Manager

public class MainMenu : MonoBehaviour
{
    // Start Game Button
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Here you need to fill in the name of the main scene of your game
    }

    // Enter the settings interface
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene"); // You need to have a "SettingsScene"
    }

    // Character Selection
    public void OpenCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelectionScene"); // Make sure "CharacterSelectionScene" exists
    }

    // Exit Game
    public void QuitGame()
    {
        Application.Quit(); // Exit the application
        Debug.Log("Game is exiting..."); // Visible only in Unity Editor Mode
    }
}
