using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Fireball_Shooter))] // Attack 1
[RequireComponent(typeof(GuidedStreamAttack))] // Attack 2
[RequireComponent(typeof(ElementalDash))] // Ability 1
[RequireComponent(typeof(WaterRing_Attack))] // Ability 2
[RequireComponent(typeof(Ultimate_Attack))] // Ultimate
public class CharacterClass : MonoBehaviour
{
    // Character class variables
    [Header("Character Properties")]
    [SerializeField] protected float health = 100.0f;

    [Header("Ability Cooldowns")]
    [SerializeField] float[] abilityCooldowns = new float[5]; // define character cooldowns
    private float[] currentCooldowns; // track cooldown statuses

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI[] AbilityCooldownTexts = new TextMeshProUGUI[5];

    Fireball_Shooter fireball;
    GuidedStreamAttack guidedStream;
    ElementalDash elementalDash;
    WaterRing_Attack waterRing;
    Ultimate_Attack ultimate;

    Animator animator;

    [Header("Elemental Prefabs")]
    public GameObject fireUlt;
    public GameObject waterUlt;
    public GameObject fireRingPrefab;
    public GameObject waterRingPrefab;
    public GameObject fireBall;
    public GameObject waterBall;
    public ParticleSystem firePs;
    public ParticleSystem waterPs;
    public GuidedStream firestream;
    public GuidedStream waterstream;

    private GameObject selectedAt1;
    private GuidedStream selectedAt2;
    private ParticleSystem selectedAb1;
    private GameObject selectedAb2;
    private GameObject selectedUlt;

    private void Awake()
    {
        // Initialize cooldown array to match the number of defined ability cooldowns.
        currentCooldowns = new float[abilityCooldowns.Length];
        
        // Try to get the Animator component.
        animator = GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject. Please add an Animator.");
        }

        // Retrieve ability references from this GameObject.
        fireball = GetComponent<Fireball_Shooter>();
        guidedStream = GetComponent<GuidedStreamAttack>();
        elementalDash = GetComponent<ElementalDash>();
        waterRing = GetComponent<WaterRing_Attack>();
        ultimate = GetComponent<Ultimate_Attack>();

        // Check tag and assign corresponding prefabs.
        switch (gameObject.tag)
        {
            case "Fire":
                selectedAt1 = fireBall;
                selectedAt2 = firestream;
                selectedAb1 = firePs;
                selectedAb2 = fireRingPrefab;
                selectedUlt = fireUlt;
                break;

            case "Water":
                selectedAt1 = waterBall;
                selectedAt2 = waterstream;
                selectedAb1 = waterPs;
                selectedAb2 = waterRingPrefab;
                selectedUlt = waterUlt;
                break;

            default:
                Debug.LogWarning("CharacterClass: Tag is not set to 'Fire' or 'Water'. Defaulting to 'Fire' settings.");
                selectedAt1 = fireBall;
                selectedAt2 = firestream;
                selectedAb1 = firePs;
                selectedAb2 = fireRingPrefab;
                selectedUlt = fireUlt;
                break;
        }
        Debug.Log("Assigning Prefabs");
        AssignPrefabs();
    }

    private void AssignPrefabs()
    {
        if (fireball != null)
            fireball.SetPrefab(selectedAt1);
        if (guidedStream != null)
            guidedStream.SetPrefab(selectedAt2);
        if (elementalDash != null)
            elementalDash.SetPrefab(selectedAb1);
        if (waterRing != null)
            waterRing.SetPrefab(selectedAb2);
        if (ultimate != null)
            ultimate.SetPrefab(selectedUlt);
    }

    void Update()
    {
        UpdateCooldowns();
    }

    public void PerformAttack1()
    {
        if (fireball != null)
        {
            fireball.Trigger();
            if (animator != null)
                animator.SetTrigger("Attack1");
        }
        else
        {
            Debug.LogError("Fireball_Shooter script is missing on the player!");
        }
    }

    public void PerformAttack2()
    {
        if (guidedStream != null)
        {
            guidedStream.Trigger();
            if (animator != null)
                animator.SetTrigger("Attack2");
        }
    }

    public void PerformAbility1()
    {
        if (elementalDash != null)
        {
            elementalDash.Trigger();
            if (animator != null)
                animator.SetBool("IsDashing", true);
        }
    }

    public void PerformAbility2()
    {
        if (animator != null)
            animator.SetTrigger("Ability2");
        if (waterRing != null)
        {
            waterRing.Trigger();
        }
    }

    public void PerformUltimate()
    {
        if (ultimate != null)
        {
            ultimate.Trigger();
        }
    }

    private void UpdateCooldowns()
    {
        if (currentCooldowns == null || currentCooldowns.Length != abilityCooldowns.Length)
        {
            Debug.LogWarning("Warning: Current cooldowns wasn't initialized correctly or differs from length of cooldown array");
            return;
        }

        for (int i = 0; i < currentCooldowns.Length; i++)
        {
            if (currentCooldowns[i] > 0.0f)
            {
                currentCooldowns[i] -= Time.deltaTime;
                // Check if the UI text for this ability is assigned before setting its text.
                if (AbilityCooldownTexts != null && i < AbilityCooldownTexts.Length && AbilityCooldownTexts[i] != null)
                    AbilityCooldownTexts[i].text = currentCooldowns[i].ToString("F1");
            }
            else
            {
                currentCooldowns[i] = 0;
                if (AbilityCooldownTexts != null && i < AbilityCooldownTexts.Length && AbilityCooldownTexts[i] != null)
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
        // Ensure the animator has at least 2 layers before using index 1.
        if (animator != null && animator.layerCount > 1)
        {
            if (!animator.GetCurrentAnimatorStateInfo(1).IsName("UpperBodyIdle"))
            {
                Debug.Log("Can't use ability while in animation");
                return;
            }
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
        if (abilityIndex < 0 || abilityIndex >= abilityCooldowns.Length)
        {
            Debug.LogWarning("Trying to access non-existent ability index.");
            return false;
        }

        return (currentCooldowns[abilityIndex] == 0);
    }

    private void ResetAbilityCooldown(int abilityIndex)
    {
        if (abilityIndex < 0 || abilityIndex >= abilityCooldowns.Length)
        {
            Debug.LogWarning("Trying to access non-existent ability index.");
            return;
        }

        currentCooldowns[abilityIndex] = abilityCooldowns[abilityIndex];
    }
}
