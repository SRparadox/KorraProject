using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SwitchSettings : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject Gameplay;
    [SerializeField] private GameObject Audio;
    [SerializeField] private GameObject Accessibility;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableMenu()
    {
        Menu.SetActive(true);
        Settings.SetActive(false);
    }
    public void EnableSettings()
    {
        Settings.SetActive(true);
        Menu.SetActive(false);
    }
    public void EnableGameplay()
    {
        Gameplay.SetActive(true);
        Audio.SetActive(false);
        Accessibility.SetActive(false);
    }
    public void EnableAudio()
    {
        Audio.SetActive(true);
        Gameplay.SetActive(false);
        Accessibility.SetActive(false);
    }
    public void EnableAccessibility()
    {
        Accessibility.SetActive(true);
        Audio.SetActive(false);
        Gameplay.SetActive(false);
    }
}
