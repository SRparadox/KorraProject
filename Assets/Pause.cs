using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool ToggleMenu;
    [SerializeField] private GameObject PauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
                ToggleMenu = !ToggleMenu;
                PauseMenu.SetActive(ToggleMenu);
                Cursor.lockState = ToggleMenu ? CursorLockMode.None : CursorLockMode.Locked;
            }
    }

    public void PauseToggle()
    {
        ToggleMenu = !ToggleMenu;
        PauseMenu.SetActive(ToggleMenu);
        Cursor.lockState = ToggleMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
