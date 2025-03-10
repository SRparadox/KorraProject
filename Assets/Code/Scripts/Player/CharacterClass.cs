using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FireballShooter))] // Attack 1
[RequireComponent(typeof(GuidedStreamAttack))] // Attack 2
[RequireComponent(typeof(ElementalDash))] // Ability 1
[RequireComponent(typeof(WaterRingAttack))] // Ability 2
[RequireComponent(typeof(UltimateAttack))] // Ultimate


public class CharacterClass: MonoBehaviour
{
    // Character class variables
    [Header("Character Properties")]
    [SerializeField] protected float health = 100.0f;
    [SerializeField] protected Slider HealthBar;
    [SerializeField] TextMeshProUGUI HealthBarText;

    [Header("Ability Cooldowns")]
    [SerializeField] float[] abilityCooldowns = new float[5]; // define character cooldowns
    private float[] currentCooldowns; // track cooldown statuses

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI[] AbilityCooldownTexts = new TextMeshProUGUI[5];

    FireballShooter fireball;
    GuidedStreamAttack guidedStream;
    ElementalDash elementalDash;
    WaterRingAttack waterRing;
    UltimateAttack ultimate;

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
    public Material waterMaterial;
    public Material fireMaterial;
    public GameObject playerBody;

    private GameObject selectedAt1;
    private GuidedStream selectedAt2;
    private ParticleSystem selectedAb1;
    private GameObject selectedAb2;
    private GameObject selectedUlt;

    private int maxAttack1Uses = 4;
    private int currentAttack1Uses;
    private int ultimateCharge = 0;
    public int maxUltimateCharge = 30;
    private bool isAttack1OnCooldown = false;

    public bool isPlayer = true;
    public GameObject textSpawnLocation;
    public GameObject dmgTextPrefab;
    private DamageBoost damageBoostScript;
    public ParticleSystem healParticles;


    private void Awake()
    {
        if (HealthBar) {
            HealthBar.value = health;
            HealthBarText.text = health.ToString() + " HP";
        }
        currentCooldowns = new float[abilityCooldowns.Length];
        animator = GetComponent<Animator>();
        currentAttack1Uses = maxAttack1Uses;
        damageBoostScript = GetComponent<DamageBoost>();

        // Retrieve ability references
        fireball = GetComponent<FireballShooter>();
        guidedStream = GetComponent<GuidedStreamAttack>();
        elementalDash = GetComponent<ElementalDash>();
        waterRing = GetComponent<WaterRingAttack>();
        ultimate = GetComponent<UltimateAttack>();

        switch (gameObject.tag)
        {
            case "Fire":
            selectedAt1 = fireBall;
            selectedAt2 = firestream;
            selectedAb1 = firePs;
            selectedAb2 = fireRingPrefab;
            selectedUlt = fireUlt;
            playerBody.GetComponent<Renderer>().material = fireMaterial;
            break;

            case "Water":
            selectedAt1 = waterBall;
            selectedAt2 = waterstream;
            selectedAb1 = waterPs;
            selectedAb2 = waterRingPrefab;
            selectedUlt = waterUlt;
            playerBody.GetComponent<Renderer>().material = waterMaterial;
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
        if (isPlayer) UpdateCooldowns();
    }

    public float getDamageMultiplier()
    {
        return damageBoostScript.getDamageBoost();
    }

    public void triggerFireball()
    {
        if (fireball != null)
        {
            fireball.Trigger();
        }
    }

    public void PerformAttack1()
    {
        if (fireball != null)
        {
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
    public void triggerRing()
    {
        if (waterRing != null)
        {
            waterRing.Trigger();
        }
    }

    public void PerformAbility2()
    {
        animator.SetTrigger("Ability2");
    }
    public void PerformUltimate()
    {
        if(ultimateCharge >= maxUltimateCharge)
        {
            if (ultimate != null)
            {
                animator.SetTrigger("Ultimate");
                ultimate.Trigger();
                ultimateCharge = 0;
                Debug.Log("Ultimate actived!");
            }
        }
        else
        {
            Debug.Log("Ultimate not ready yet");
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
            if(i == 4)
            {
                float ultimatePercentage = (float)ultimateCharge / maxUltimateCharge * 100f;
                AbilityCooldownTexts[i].text = $"{ultimatePercentage:F0}%";
                if(ultimatePercentage == 100)
                {
                    AbilityCooldownTexts[i].text = "Ready";
                }
                continue;
            }
            if (currentCooldowns[i] > 0.0f)
            {
                currentCooldowns[i] -= Time.deltaTime;
                AbilityCooldownTexts[i].text = currentCooldowns[i].ToString("F1");
            } else
            {
                currentCooldowns[i] = 0;
                AbilityCooldownTexts[i].text = "Ready";

                if (i == 0 && isAttack1OnCooldown)
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
        if ((animator.GetCurrentAnimatorStateInfo(1).IsName("RightPunch") || animator.GetCurrentAnimatorStateInfo(1).IsName("LeftPunch")) && abilityIndex == 0 && animator.GetLayerWeight(1) >= 0.7f)
        {
            ResetAbilityCooldown(abilityIndex);
            animator.SetBool("BufferPunch", true);
            return;
        }
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("UpperBodyIdle") || animator.GetLayerWeight(1) < 0.7f)
        {
            Debug.Log("Can't use ability while in animation");
            //Maybe play a audio cue here
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

    bool canTakeDamage = true;
    IEnumerator ResetDamageCooldown()
    {
        Debug.Log("Damage cooldown started");
        yield return new WaitForSeconds(0.25f);
        canTakeDamage = true;
    }

    void spawnDamageText(float damage)
    {
        GameObject dmgText = Instantiate(dmgTextPrefab, textSpawnLocation.transform.position, Quaternion.identity);
        dmgText.GetComponent<DamageText>().setDamageText(damage);
    }
    public ParticleSystem takeDamageParticles;

    public void TakeDamage(float damage)
    {
        if (!canTakeDamage) return;
        Debug.Log("Player has taken " + damage + " damage.");
        health -= damage;
        if (takeDamageParticles != null)
        {
            takeDamageParticles.Play();
        }
        spawnDamageText(damage);
        canTakeDamage = false;
        StartCoroutine(ResetDamageCooldown());
        if (health <= 0)
        {
            Respawn();
        }
        if (HealthBar) {
            HealthBar.value = health;
            HealthBarText.text = health.ToString() + " HP";
        }
    }

    public void Respawn(){
        health = 100;
        //Add respawn mechanic here
    }

    public void Heal(float amount)
    {
        Debug.Log("Player has healed " + amount + " health.");
        if (healParticles != null)
        {
            healParticles.Play();
        }
        health = Mathf.Min(health + amount, 100);
    }

    public void OnSuccessfulHit()
    {
        ultimateCharge = Mathf.Min(ultimateCharge + 1, maxUltimateCharge);
        if(ultimateCharge >= maxUltimateCharge)
        {
            Debug.Log("Ultimate is ready!");
        }
    }

}
