using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool ToggleMenu;
    public bool ToggleAbilityButtons;

    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject AbilityButtons;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private Slider MusicVolume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
                HUD.SetActive(ToggleMenu);
                AbilityButtons.SetActive(ToggleMenu);
                ToggleMenu = !ToggleMenu;
                PauseMenu.SetActive(ToggleMenu);
                SettingsMenu.SetActive(false);
                Cursor.lockState = ToggleMenu ? CursorLockMode.None : CursorLockMode.Locked;
        }

        // in case the player leaves and reenters application while paused
        if ((PauseMenu.activeSelf || SettingsMenu.activeSelf) && Cursor.lockState == CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.None;
        }
        if ((!PauseMenu.activeSelf && !SettingsMenu.activeSelf) && ToggleAbilityButtons) {
            AbilityButtons.SetActive(false);
        }
    }

    public void PauseToggle()
    {
        HUD.SetActive(ToggleMenu);
        AbilityButtons.SetActive(ToggleMenu);
        ToggleMenu = !ToggleMenu;
        PauseMenu.SetActive(ToggleMenu);
        Cursor.lockState = ToggleMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void AbilityButtonsToggle()
    {
        ToggleAbilityButtons = !ToggleAbilityButtons;
    }
    public void AudioVolume()
    {
        AudioSource.volume = MusicVolume.value;
    }
}
