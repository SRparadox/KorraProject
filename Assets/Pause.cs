using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool ToggleMenu;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject HUD;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
                HUD.SetActive(ToggleMenu);
                ToggleMenu = !ToggleMenu;
                PauseMenu.SetActive(ToggleMenu);
                Cursor.lockState = ToggleMenu ? CursorLockMode.None : CursorLockMode.Locked;
        }

        // in case the player leaves and reenters application while paused
        if (PauseMenu.activeSelf && Cursor.lockState == CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void PauseToggle()
    {
        HUD.SetActive(ToggleMenu);
        ToggleMenu = !ToggleMenu;
        PauseMenu.SetActive(ToggleMenu);
        Cursor.lockState = ToggleMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
