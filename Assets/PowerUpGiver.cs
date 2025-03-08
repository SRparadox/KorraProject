using System.Collections;
using StarterAssets;
using UnityEngine;

public class PowerUpGiver : MonoBehaviour
{
    public enum PowerUpType
    {
        Speed,
        Damage,
        Health
    }
    [Header("PowerUp Type")]
    public PowerUpType powerUpType;
    public bool isActive = true;

    [Header("Materials")]
    public Material speedMaterial;
    public Material speedMaterial2;
    public Material damageMaterial;
    public Material damageMaterial2;
    public Material healthMaterial;
    public Material healthMaterial2;
    public GameObject lightEffect;
    public GameObject icon;
    private GameObject scroll;
    private Collider col;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider>();
        scroll = GameObject.Find("Scroll");
        if (scroll == null)
        {
            Debug.LogError("Scroll GameObject not found in the scene.");
        }
        else
        {
            scroll.SetActive(false);
        }
        setupObject();
    }

    public void setupObject(){
        if (isActive)
        {
            setupMaterial();
            scroll.SetActive(true);
            lightEffect.SetActive(true);
            icon.SetActive(true);
            col.enabled = true;
        }
        else
        {
            col.enabled = false;
            scroll.SetActive(false);
            lightEffect.SetActive(false);
            icon.SetActive(false);
        }
    }

    void setupMaterial()
    {
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                lightEffect.GetComponent<Renderer>().material = speedMaterial;
                icon.GetComponent<Renderer>().material = speedMaterial2;
                break;
            case PowerUpType.Damage:
                lightEffect.GetComponent<Renderer>().material = damageMaterial;
                icon.GetComponent<Renderer>().material = damageMaterial2;
                break;
            case PowerUpType.Health:
                lightEffect.GetComponent<Renderer>().material = healthMaterial;
                icon.GetComponent<Renderer>().material = healthMaterial2;
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            Debug.Log("PowerUpGiver: " + powerUpType);
            switch (powerUpType)
            {
                case PowerUpType.Speed:
                    player.activateSpeedPowerup();
                    break;
                case PowerUpType.Damage:
                    player.GetComponent<DamageBoost>().ActivateBoost();
                    break;
                case PowerUpType.Health:
                    player.GetComponent<CharacterClass>().Heal(25);
                    break;
            }
            isActive = false;
            setupObject();
        }
    }

    IEnumerator respawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isActive = true;
        setupObject();
    }


    public void spawnPowerUp(){
        PowerUpType type = (PowerUpType)Random.Range(0, 3);
        Debug.Log("PowerUpGiver: " + type);
        powerUpType = type;
        isActive = true;
        setupObject();
    }


}
