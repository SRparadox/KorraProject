using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class HandleAbilityCDOnLocalPlayer : MonoBehaviour
{
    // Reference to our local player's GameObject (assigned at runtime).
    public GameObject localPlayer;
    
    [SerializeField] private TextMeshProUGUI Attack1CDText;
    [SerializeField] private TextMeshProUGUI Attack2CDText;
    [SerializeField] private TextMeshProUGUI Ability1CDText;
    [SerializeField] private TextMeshProUGUI Ability2CDText;
    [SerializeField] private TextMeshProUGUI UltimateCDText;
    [SerializeField] private Image[] abilitySelectedImage;
    private int selectedAbility = 0;
    [SerializeField] private Toggle oneHandedModeToggle;

    [SerializeField] protected Slider HealthBar;
    [SerializeField] protected Slider SensitivitySlider;
    [SerializeField] private TextMeshProUGUI HealthBarText;

    void Start()
    {
        // Instead of relying on a "KeepTrackOfPlayer" object,
        // loop through all spawned network objects and find the one owned by this client.
        if (NetworkManager.Singleton != null)
        {
            foreach (var kvp in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
            {
                
                if (kvp.Value.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    if (kvp.Value.gameObject == null)
                    {
                        Debug.LogError("HandleAbilityCDOnLocalPlayer: Local player object is null!");
                        break;
                    }
                    localPlayer = kvp.Value.gameObject;
                    break;
                }
            }
        }
        if (localPlayer == null)
        {
            Debug.LogError("HandleAbilityCDOnLocalPlayer: Local player not found!");
            return;
        }
        
        // Apply sensitivity from the slider to the local player's ThirdPersonController.
        ThirdPersonController controller = localPlayer.GetComponent<ThirdPersonController>();
        if (controller != null)
        {
            controller.updateSensitive(SensitivitySlider.value);
        }
        else
        {
            Debug.LogError("HandleAbilityCDOnLocalPlayer: ThirdPersonController not found on local player!");
        }

        // Set the color for ability selection images based on the player's team.
        CharacterClass character = localPlayer.GetComponent<CharacterClass>();
        if (character != null)
        {
            Color color = character.getTeamColor();
            for (int i = 0; i < abilitySelectedImage.Length; i++)
            {
                abilitySelectedImage[i].enabled = false;
                abilitySelectedImage[i].color = color;
            }
        }
        else
        {
            Debug.LogError("HandleAbilityCDOnLocalPlayer: CharacterClass not found on local player!");
        }
        
        // If using one-handed mode, select the current ability.
        StarterAssetsInputs inputs = localPlayer.GetComponent<StarterAssetsInputs>();
        if (inputs != null && inputs.isOneHanded())
        {
            SelectAbility(selectedAbility);
        }
    }

    void updateCooldowns()
    {
        if (localPlayer == null)
            return;
            
        CharacterClass playerClass = localPlayer.GetComponent<CharacterClass>();
        if (playerClass == null)
            return;
            
        Attack1CDText.text = playerClass.getTextForCD(0);
        Attack2CDText.text = playerClass.getTextForCD(1);
        Ability1CDText.text = playerClass.getTextForCD(2);
        Ability2CDText.text = playerClass.getTextForCD(3);
        UltimateCDText.text = playerClass.getTextForCD(4);
    }

    public void SelectAbility(int abilityIndex)
    {
        if (localPlayer == null)
            return;
        
        hideSelectedAbilities();
        if (abilityIndex >= 0 && abilityIndex < abilitySelectedImage.Length)
        {
            abilitySelectedImage[abilityIndex].enabled = true;
        }
    }
    
    private void hideSelectedAbilities()
    {
        for (int i = 0; i < abilitySelectedImage.Length; i++)
        {
            abilitySelectedImage[i].enabled = false;
        }
    }

    void updateHealthBar()
    {
        if (localPlayer == null)
            return;
            
        CharacterClass character = localPlayer.GetComponent<CharacterClass>();
        if (character == null)
            return;
            
        float health = character.getHealth();
        HealthBar.value = health;
        HealthBarText.text = health + " / 100";
    }

    public void updateSensitivityValue()
    {
        if (localPlayer == null)
            return;
            
        ThirdPersonController controller = localPlayer.GetComponent<ThirdPersonController>();
        if (controller != null)
            controller.updateSensitive(SensitivitySlider.value);
    }

    public void SwitchTeams()
    {
        if (localPlayer == null)
            return;
            
        CharacterClass character = localPlayer.GetComponent<CharacterClass>();
        if (character != null)
            character.SwitchTeams();
    }

    public void toggleOneHandedMode()
    {
        if (localPlayer == null)
            return;

        StarterAssetsInputs inputs = localPlayer.GetComponent<StarterAssetsInputs>();
        if (inputs != null)
            inputs.setOneHanded(oneHandedModeToggle.isOn);

        if (oneHandedModeToggle.isOn)
        {
            SelectAbility(selectedAbility);
        }
        else
        {
            hideSelectedAbilities();
        }
    }

    void Update()
    {
        if (localPlayer == null)
            return;
        updateCooldowns();
        updateHealthBar();
        
        StarterAssetsInputs inputs = localPlayer.GetComponent<StarterAssetsInputs>();
        if (inputs != null && selectedAbility != inputs.selectedAbility)
        {
            selectedAbility = inputs.selectedAbility;
            SelectAbility(selectedAbility);
        }
    }
}
