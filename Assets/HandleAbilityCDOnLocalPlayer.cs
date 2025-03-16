using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleAbilityCDOnLocalPlayer : MonoBehaviour
{
    public GameObject localPlayer;
    private PlayerTracker playerTracker;
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
    [SerializeField] TextMeshProUGUI HealthBarText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerTrackerObj = GameObject.Find("KeepTrackOfPlayer");
        if (playerTrackerObj == null)
        {
            Debug.LogError("PlayerTracker not found");
        
        }
        else {
            playerTracker = playerTrackerObj.GetComponent<PlayerTracker>();
            localPlayer = playerTracker.GetPlayer();
            oneHandedModeToggle.isOn = playerTracker.isOneHanded;
        }
        
        // line of code below was added to allow player settings to carry across scenes
        localPlayer.GetComponent<ThirdPersonController>().updateSensitive(SensitivitySlider.value);

        Color color = localPlayer.GetComponent<CharacterClass>().getTeamColor();
        for (int i = 0; i < abilitySelectedImage.Length; i++){
            abilitySelectedImage[i].enabled = false;
            abilitySelectedImage[i].color = color;
        }
        
        if (localPlayer.GetComponent<StarterAssetsInputs>().isOneHanded()){
            SelectAbility(selectedAbility);
        }

    }

    void updateCooldowns(){
        if (localPlayer == null) return;
        CharacterClass playerClass = localPlayer.GetComponent<CharacterClass>();
        Attack1CDText.text = playerClass.getTextForCD(0);
        Attack2CDText.text = playerClass.getTextForCD(1);
        Ability1CDText.text = playerClass.getTextForCD(2);
        Ability2CDText.text = playerClass.getTextForCD(3);
        UltimateCDText.text = playerClass.getTextForCD(4);
    }

    public void SelectAbility(int abilityIndex){
        if (localPlayer == null) return;
        
        hideSelectedAbilitys();
        abilitySelectedImage[abilityIndex].enabled = true;
    }
    private void hideSelectedAbilitys(){
        for (int i = 0; i < abilitySelectedImage.Length; i++){
            abilitySelectedImage[i].enabled = false;
        }
    }

    void updateHealthBar(){
        if (localPlayer == null) return;
        HealthBar.value = localPlayer.GetComponent<CharacterClass>().getHealth();
        HealthBarText.text = localPlayer.GetComponent<CharacterClass>().getHealth() + " HP";
    }

    public void updateSensitivityValue(){
        if (localPlayer == null) return;
        localPlayer.GetComponent<ThirdPersonController>().updateSensitive(SensitivitySlider.value);
    }

    public void SwitchTeams(){
        if (localPlayer == null) return;
        localPlayer.GetComponent<CharacterClass>().SwitchTeams();
    }

    public void toggleOneHandedMode(){
        if (localPlayer == null) return;
   
        if (playerTracker != null) playerTracker.setOneHanded(oneHandedModeToggle.isOn);

        localPlayer.GetComponent<StarterAssetsInputs>().setOneHanded(oneHandedModeToggle.isOn);
        if (oneHandedModeToggle.isOn){
            SelectAbility(selectedAbility);
        } else {
            hideSelectedAbilitys();
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateCooldowns();
        updateHealthBar();
        if (selectedAbility != localPlayer.GetComponent<StarterAssetsInputs>().selectedAbility){
            selectedAbility = localPlayer.GetComponent<StarterAssetsInputs>().selectedAbility;
            SelectAbility(selectedAbility);
        }
    }
}
