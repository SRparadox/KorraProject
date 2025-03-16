using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool ToggleMenu;
    public bool ToggleAbilityButtons;
    public ReadyUpManager readyUpManager;

    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private GameObject CreditsMenu;

    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject AbilityButtons;
    [SerializeField] private AudioSource AudioSource;
    
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Slider SensitivitySlider;

    public static float CurrentSensitivity = 4;
    public static float CurrentVolume = 7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        SensitivitySlider.value = CurrentSensitivity;
        MusicVolume.value = CurrentVolume;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentSensitivity = SensitivitySlider.value;
        CurrentVolume = MusicVolume.value;
        if(Input.GetKeyDown(KeyCode.Escape)) {
                HUD.SetActive(ToggleMenu);
                AbilityButtons.GetComponent<Canvas>().enabled = ToggleMenu;
                ToggleMenu = !ToggleMenu;
                PauseMenu.SetActive(ToggleMenu);
                SettingsMenu.SetActive(false);
                CreditsMenu.SetActive(false);
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

    public void OpenSettings()
    {
        SettingsMenu.SetActive(true);
        PauseMenu.SetActive(false);

        if (readyUpManager != null)
            readyUpManager.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseSettings()
    {
        SettingsMenu.SetActive(false);
        PauseMenu.SetActive(true);
        if (readyUpManager != null)
            readyUpManager.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
