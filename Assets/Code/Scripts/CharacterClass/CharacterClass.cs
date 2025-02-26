using TMPro;
using UnityEngine;

[RequireComponent(typeof(Fireball_Shooter))] // Attack 1
[RequireComponent(typeof(GuidedStreamAttack))] // Attack 2
[RequireComponent(typeof(ElementalDash))] // Ability 1
[RequireComponent(typeof(WaterRing_Attack))] // Ability 2
[RequireComponent(typeof(Ultimate_Attack))] // Ultimate


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

    private int maxAttack1Uses = 4;
    private int currentAttack1Uses;
    private bool isAttack1OnCooldown = false;


    private void Awake()
    {
        currentCooldowns = new float[abilityCooldowns.Length];
        animator = GetComponent<Animator>();
        currentAttack1Uses = maxAttack1Uses;

        // Retrieve ability references
        fireball = GetComponent<Fireball_Shooter>();
        guidedStream = GetComponent<GuidedStreamAttack>();
        elementalDash = GetComponent<ElementalDash>();
        waterRing = GetComponent<WaterRing_Attack>();
        ultimate = GetComponent<Ultimate_Attack>();

        switch(gameObject.tag)
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
        }

        AssignPrefabs();
    }

    private void AssignPrefabs()
    {
        if (fireball != null) fireball.SetPrefab(selectedAt1);
        if (guidedStream != null) guidedStream.SetPrefab(selectedAt2);
        if (elementalDash != null) elementalDash.SetPrefab(selectedAb1);
        if (waterRing != null) waterRing.SetPrefab(selectedAb2);
        if (ultimate != null) ultimate.SetPrefab(selectedUlt);
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
            animator.SetTrigger("Attack1");
        }
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
        if(waterRing != null)
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
                AbilityCooldownTexts[i].text = currentCooldowns[i].ToString("F1");
            } else
            {
                currentCooldowns[i] = 0;
                AbilityCooldownTexts[i].text = "Ready";

                if(i == 0 && isAttack1OnCooldown)
                {
                    currentAttack1Uses = maxAttack1Uses;
                    isAttack1OnCooldown = false;
                    Debug.Log("Attack 1 shots reset after cooldown.");
                }
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
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("UpperBodyIdle"))
        {
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
