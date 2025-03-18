using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene: MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }
    public void SwitchToPlayScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("PlayScene");
    }
    public void SwitchToPlaySceneFire()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Change scene name here when ready
    
        SceneManager.LoadScene("PlayScene");

    }
    public void SwitchToCreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }
    public void SwitchToSettingsScene()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    public void SwitchToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void SwitchToCharacterSelectionScene()
    {
        SceneManager.LoadScene("CharacterSelectionScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
