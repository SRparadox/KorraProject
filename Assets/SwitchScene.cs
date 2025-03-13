using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    private PlayerTracker playerTracker;

    void Start()
    {
        GameObject keepTrackObject = GameObject.Find("KeepTrackOfPlayer");
        if (keepTrackObject != null)
        {
            playerTracker = keepTrackObject.GetComponent<PlayerTracker>();
            if (playerTracker == null)
            {
                Debug.LogError("SwitchScene: PlayerTracker component not found on 'KeepTrackOfPlayer'!");
            }
        }
        else
        {
            Debug.LogError("SwitchScene: 'KeepTrackOfPlayer' object not found in the scene!");
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
        if (playerTracker != null)
        {
            playerTracker.SetPlayerTeam(CharacterClass.PlayerTeam.Fire);
        }
        else
        {
            Debug.LogWarning("SwitchToPlaySceneFire: PlayerTracker is null; cannot set player team.");
        }
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
