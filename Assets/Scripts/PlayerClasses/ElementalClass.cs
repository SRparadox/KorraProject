using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/* 
Elements of a Character Base Class
- Properties: health, speed, jump height, etc.
- Retrieve/handle input
- Handle Cooldowns
- Destructor
*/

/*
Potential issues:
- Ability cooldown text array could differ in length to # of cooldowns --> potential tuntime error in UpdateCooldowns();
- 


*/

public abstract class ElementalClass: MonoBehaviour
{
    // Character class variables
    [Category("Character Properties")]
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float jumpHeight = 5f;

    [Category("Ability Cooldowns")]
    [SerializeField] float[] AbilityCooldowns = new float[5]; // define character cooldowns
    private float[] currentCooldowns; // track cooldown statuses

    [Category("UI Elements")]
    [SerializeField] TextMeshProUGUI[] AbilityCooldownTexts = new TextMeshProUGUI[5];

    void Start()
    {
        currentCooldowns = new float[AbilityCooldowns.Length];
    }

    void Update()
    {
        UpdateCooldowns();
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < currentCooldowns.Length; i++)
        {
            if (currentCooldowns[i] > 0.0f)
            {
                currentCooldowns[i] -= Time.deltaTime;
                AbilityCooldownTexts[i].text = currentCooldowns[i].ToString("F1");
            } else
            {
                currentCooldowns[i] = 0;
                AbilityCooldownTexts[i].text = "Ready";
            }
        }
    }

    public void UseAbility(int abilityIndex)
    {
        if (abilityIndex < 0 || abilityIndex >= currentCooldowns.Length)
        {
            Debug.LogWarning("Trying to access non-existent ability index.");
            return;
        }

        if (IsAbilityReady(abilityIndex))
        {
            switch (abilityIndex)
            {
                case 0:
                PerformAttack1();
                break;
                case 1:
                PerformAttack2();
                break;
                case 2:
                PerformAbility1();
                break;
                case 3:
                PerformAbility2();
                break;
                case 4:
                PerformUltimate();
                break;
            }

            ResetAbilityCooldown(abilityIndex);
        }
    }

    // Helpers
    private bool IsAbilityReady(int abilityIndex)
    {
        return (currentCooldowns[abilityIndex] == 0);
    }

    private void ResetAbilityCooldown(int abilityIndex)
    {
        currentCooldowns[abilityIndex] = AbilityCooldowns[abilityIndex];
    }

    public abstract void PerformAttack1();
    public abstract void PerformAttack2();
    public abstract void PerformAbility1();
    public abstract void PerformAbility2();
    public abstract void PerformUltimate();
}
