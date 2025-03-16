using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SwitchSettings: MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject Credits;
    [SerializeField] private GameObject Tutorial;

    [SerializeField] private GameObject Gameplay;
    [SerializeField] private GameObject Audio;
    [SerializeField] private GameObject Accessibility;
    public void EnableMenu()
    {
        Menu.SetActive(true);
        Settings.SetActive(false);
        Credits.SetActive(false);
        Tutorial.SetActive(false);
    }
    public void EnableSettings()
    {
        Settings.SetActive(true);
        Menu.SetActive(false);
        Credits.SetActive(false);
        Tutorial.SetActive(false);
    }
    public void EnableCredits()
    {
        Credits.SetActive(true);
        Settings.SetActive(false);
        Menu.SetActive(false);
        Tutorial.SetActive(false);
    }
    public void EnableTutorial()
    {
        Tutorial.SetActive(true);
        Settings.SetActive(false);
        Menu.SetActive(false);
        Credits.SetActive(false);
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
