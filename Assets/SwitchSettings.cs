using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SwitchSettings : MonoBehaviour
{
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
