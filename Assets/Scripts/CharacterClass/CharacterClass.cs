using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GuidedStreamAttack))] // Attack 2
[RequireComponent(typeof(ElementalDash))] // Ability 1

public class CharacterClass: MonoBehaviour
{
    // Character class variables
    [Header("Character Properties")]
    [SerializeField] protected float health = 100.0f;
    [SerializeField] protected float speed = 20.0f;
    [SerializeField] protected float jumpHeight = 5.0f;

    [Header("Ability Cooldowns")]
    [SerializeField] float[] abilityCooldowns = new float[5]; // define character cooldowns
    private float[] currentCooldowns; // track cooldown statuses

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI[] AbilityCooldownTexts = new TextMeshProUGUI[5];

    GuidedStreamAttack guidedStream;
    ElementalDash elementalDash;

    Animator animator;

    private void Awake()
    {
        currentCooldowns = new float[abilityCooldowns.Length];
        animator = GetComponent<Animator>();

        // Retrieve ability references
        guidedStream = GetComponent<GuidedStreamAttack>();
        elementalDash = GetComponent<ElementalDash>();
    }

    void Update()
    {
        UpdateCooldowns();
    }

    public void PerformAttack1()
    {
        animator.SetTrigger("Attack1");
    }

    public void PerformAttack2()
    {
        if (guidedStream != null)
        {
            guidedStream.Trigger();
            animator.SetTrigger("Attack2");
        }
    }
    public void PerformAbility1()
    {
        if (guidedStream != null)
        {
            elementalDash.Trigger();
            animator.SetBool("IsDashing", true);
        }
    }
    public void PerformAbility2()
    {

    }
    public void PerformUltimate()
    {

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
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("UpperBodyIdle")){
            Debug.Log("Can't use ability while in animation");
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
