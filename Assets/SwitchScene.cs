using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToPlayScene()
    {
        SceneManager.LoadScene("SampleScene");
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
