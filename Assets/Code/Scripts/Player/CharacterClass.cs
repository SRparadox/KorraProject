using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FireballShooter))] // Attack 1
[RequireComponent(typeof(GuidedStreamAttack))] // Attack 2
[RequireComponent(typeof(ElementalDash))] // Ability 1
[RequireComponent(typeof(WaterRingAttack))] // Ability 2
[RequireComponent(typeof(UltimateAttack))] // Ultimate


public class CharacterClass: NetworkBehaviour
{

    public enum PlayerTeam
    {
        Fire,
        Water
    };

    // Character class variables
    [Header("Character Properties")]
    public NetworkVariable<PlayerTeam> team = new NetworkVariable<PlayerTeam>(
        PlayerTeam.Fire,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [SerializeField] protected float health = 50.0f;
    [SerializeField] protected float maxHealth = 100.0f;

    [Header("Ability Cooldowns")]
    [SerializeField] float[] abilityCooldowns = new float[5]; // define character cooldowns
    public float[] currentCooldowns; // track cooldown statuses

    [Header("UI Elements")]
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
    private float ultimateCharge = 0f;
    public int maxUltimateCharge = 30;
    private bool isAttack1OnCooldown = false;

    public bool isPlayer = true;
    public GameObject textSpawnLocation;
    public GameObject dmgTextPrefab;
    private DamageBoost damageBoostScript;
    public ParticleSystem healParticles;
    private GameManager GameManager;


    private void Awake()
    {
        currentCooldowns = new float[abilityCooldowns.Length];
        animator = GetComponent<Animator>();
        currentAttack1Uses = maxAttack1Uses;
        damageBoostScript = GetComponent<DamageBoost>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Retrieve ability references
        fireball = GetComponent<FireballShooter>();
        guidedStream = GetComponent<GuidedStreamAttack>();
        elementalDash = GetComponent<ElementalDash>();
        waterRing = GetComponent<WaterRingAttack>();
        ultimate = GetComponent<UltimateAttack>();


        setupAbilities();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            AssignTeam();
        }

        team.OnValueChanged += OnTeamChanged; // Ensure team updates correctly on all clients
        OnTeamChanged(PlayerTeam.Fire, team.Value); // Call once to set up the abilities
    }

    private void OnTeamChanged(PlayerTeam previousValue, PlayerTeam newValue)
    {
        setupAbilities();
        Respawn();
    }

    private void setupAbilities()
    {
        switch (getPlayersTeam())
        {
            case PlayerTeam.Fire:
            selectedAt1 = fireBall;
            selectedAt2 = firestream;
            selectedAb1 = firePs;
            selectedAb2 = fireRingPrefab;
            selectedUlt = fireUlt;
            playerBody.GetComponent<Renderer>().material = fireMaterial;
            break;

            case PlayerTeam.Water:
            selectedAt1 = waterBall;
            selectedAt2 = waterstream;
            selectedAb1 = waterPs;
            selectedAb2 = waterRingPrefab;
            selectedUlt = waterUlt;
            playerBody.GetComponent<Renderer>().material = waterMaterial;
            break;
        }
        AssignPrefabs();
    }

    [ServerRpc]
    public void SetTeamServerRpc(PlayerTeam newTeam)
    {
        if (!IsServer)
            return;
        team.Value = newTeam;

    }

    private void AssignTeam()
    {
        if (GameManager.firePlayerCount.Value <= GameManager.waterPlayerCount.Value)
        {
            team.Value = PlayerTeam.Fire;
            GameManager.IncrementFirePlayerCount();
        } else
        {
            team.Value = PlayerTeam.Water;
            GameManager.IncrementWaterPlayerCount();
        }
    }

    public Color getTeamColor()
    {
        return team.Value == PlayerTeam.Fire ? Color.red : Color.blue;
    }
    public PlayerTeam getPlayersTeam()
    {
        return team.Value;
    }
    public PlayerTeam getEnemyTeam()
    {
        //return the opposite of team
        return team.Value == PlayerTeam.Fire ? PlayerTeam.Water : PlayerTeam.Fire;
    }
    public void setPlayersTeam(PlayerTeam newTeam)
    {
        if (team.Value == newTeam)
            return;
        team.Value = newTeam;
        Respawn();
        setupAbilities();
    }
    public void SwitchTeams()
    {
        if (IsServer)
        {
            team.Value = team.Value == PlayerTeam.Fire ? PlayerTeam.Water : PlayerTeam.Fire;
        } else
        {
            SetTeamServerRpc(team.Value == PlayerTeam.Fire ? PlayerTeam.Water : PlayerTeam.Fire);
        }
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
        if (isPlayer)
            UpdateCooldowns();
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
            //increase ult charge by 5%
            ultimateCharge = ultimateCharge + (maxUltimateCharge * 0.05f);
            if (ultimateCharge > maxUltimateCharge)
            {
                ultimateCharge = maxUltimateCharge;
            }
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
            ultimateCharge = ultimateCharge + (maxUltimateCharge * 0.1f);
            if (ultimateCharge > maxUltimateCharge)
            {
                ultimateCharge = maxUltimateCharge;
            }
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
            ultimateCharge = ultimateCharge + (maxUltimateCharge * 0.15f);
            if (ultimateCharge > maxUltimateCharge)
            {
                ultimateCharge = maxUltimateCharge;
            }
        }
    }

    public ulong getPlayerID()
    {
        if (!IsOwner) return 0;
        return NetworkManager.LocalClientId;
    }

    public void PerformAbility2()
    {
        animator.SetTrigger("Ability2");
    }
    public void PerformUltimate()
    {
        if (ultimateCharge >= maxUltimateCharge)
        {
            if (ultimate != null)
            {
                animator.SetTrigger("Ultimate");
                ultimate.Trigger();
                ultimateCharge = 0;
                Debug.Log("Ultimate actived!");
            }
        } else
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
            if (i == 4)
            {
                continue;
            }
            if (currentCooldowns[i] > 0.0f)
            {
                currentCooldowns[i] -= Time.deltaTime;

            } else
            {
                currentCooldowns[i] = 0;

                if (i == 0 && isAttack1OnCooldown)
                {
                    currentAttack1Uses = maxAttack1Uses;
                    isAttack1OnCooldown = false;
                    Debug.Log("Attack 1 shots reset after cooldown.");
                }
            }
        }
    }

    public string getTextForCD(int index)
    {
        if (index < 0 || index >= abilityCooldowns.Length)
        {
            Debug.LogWarning("Trying to access non-existent ability index.");
            return "";
        }
        if (index == 4)
        {
            float ultimatePercentage = (float) ultimateCharge / maxUltimateCharge * 100f;
            return $"{ultimatePercentage:F0}%";
        }
        string text = currentCooldowns[index] > 0 ? currentCooldowns[index].ToString("F1") + "s" : "Ready";
        return text;
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
        if (abilityIndex == 4)
        {
            return (ultimateCharge >= maxUltimateCharge);
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
        if (!canTakeDamage)
            return;
        ulong clientId = NetworkManager.LocalClientId;
        if (IsOwner)
            TakeDamageServerRpc(damage, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage, ulong playerID)
    {
        if (!IsServer)
            return;
        if (!canTakeDamage)
            return;
        health -= damage;
        Debug.Log($"[Server] {gameObject.name} took {damage} damage. New health: {health}");

        TakeDamageClientRpc(health, playerID);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float newHealth, ulong playerID)
    {
        if (playerID == NetworkManager.LocalClientId)
        {
            health = newHealth;
            Debug.Log($"[Client] {gameObject.name} updated health: {health}");
            StartCoroutine(ResetDamageCooldown());
        }
        takeDamageParticles.Play();

        if (!IsOwner)
            return; // Only the owner runs this
        if (health <= 0)
        {
            Respawn();
        }
        canTakeDamage = false;
    }

    public float getHealth()
    {
        return health;
    }

    public void Respawn()
    {
        health = maxHealth;
        GameManager.RespawnPlayer(gameObject);
    }

    public void Heal(float amount)
    {
        Debug.Log("Player has healed " + amount + " health.");
        if (healParticles != null)
        {
            healParticles.Play();
        }
        health = Mathf.Min(health + amount, maxHealth);
    }
    [SerializeField] private AudioSource hitsound;
    public void OnSuccessfulHit(ulong playerID)
    {
        if (!IsOwner) return; // Only the owner runs this

        OnSuccesfulHitServerRpc(playerID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnSuccesfulHitServerRpc(ulong playerID)
    {
        if (!IsServer) return;
        OnSuccessfulHitClientRpc(playerID);
    }
    [ClientRpc]
    public void OnSuccessfulHitClientRpc(ulong playerID)
    {
        if (playerID != NetworkManager.LocalClientId) return;
        if (!IsOwner)
            return; // Only the owner runs this
        ultimateCharge = Mathf.Min(ultimateCharge + 1, maxUltimateCharge);
        hitsound.Play();
    }

}
