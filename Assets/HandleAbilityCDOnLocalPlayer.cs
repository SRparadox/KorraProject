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

    

    // Update is called once per frame
    void Update()
    {
        updateCooldowns();
        updateHealthBar();
    }
}
