using StarterAssets;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PowerUpGiver: NetworkBehaviour
{
    public enum PowerUpType
    {
        Speed,
        Damage,
        Health
    }

    [Header("PowerUp Type")]
    public NetworkVariable<PowerUpType> powerUpType = new NetworkVariable<PowerUpType>(PowerUpType.Speed);
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true);

    [Header("Materials")]
    public Material speedMaterial;
    public Material speedMaterial2;
    public Material damageMaterial;
    public Material damageMaterial2;
    public Material healthMaterial;
    public Material healthMaterial2;
    public GameObject lightEffect;
    public GameObject icon;
    [SerializeField] private GameObject scroll;
    [SerializeField] private Collider col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scroll.SetActive(false);
        setupObject();
    }

    public override void OnNetworkSpawn()
    {
        isActive.OnValueChanged += (prev, newValue) => setupObject();
        powerUpType.OnValueChanged += (prev, newValue) => setupMaterial();
        setupObject();  // Ensure initial setup on spawn
    }

    public void setupObject()
    {
        bool active = isActive.Value;
        col.enabled = active;
        scroll.SetActive(active);
        lightEffect.SetActive(active);
        icon.SetActive(active);

        if (active)
        {
            setupMaterial();
        }
    }

    void setupMaterial()
    {
        switch (powerUpType.Value)
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
        if (!isActive.Value) return;
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            switch (powerUpType.Value)
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
            SetActiveServerRpc(false);
            setupObject();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetActiveServerRpc(bool newState)
    {
        isActive.Value = newState;
    }

    IEnumerator respawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isActive.Value = true;
        setupObject();
    }

    public void spawnPowerUp()
    {
        powerUpType.Value = (PowerUpType)Random.Range(0, 3);
        isActive.Value = true;
    }
}
