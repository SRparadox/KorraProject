using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene: MonoBehaviour
{

    private PlayerTracker playerTracker;
    // Start is called before the first frame update
    void Start()
    {
        playerTracker = GameObject.Find("KeepTrackOfPlayer").GetComponent<PlayerTracker>();
        if (playerTracker == null)
        {
            Debug.LogError("PlayerTracker not found");
        }
    }
    public void SwitchToPlayScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("SampleScene");
    }
    public void SwitchToPlaySceneFire()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Change scene name here when ready
        playerTracker.SetPlayerTeam(CharacterClass.PlayerTeam.Fire);
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
