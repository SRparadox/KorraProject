using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Ultimate: NetworkBehaviour
{
    public GameObject fireRingPrefab;
    public GameObject waterRingPrefab;
    public float expansionTime = 5f;
    public float maxScale = 8f;
    public float Speed = 20f;
    public float lifeTime = 5f;
    public float damage = 75;

    [SerializeField] private NetworkVariable<float> currentScale = new NetworkVariable<float>(0f, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private CharacterClass.PlayerTeam playerTeam;
    NetworkVariable<ulong> playerID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float currentTime = 0f;
    private bool hasExpanded = false;
    private Rigidbody rb;
    private Vector3 launchDirection;
    private UltimateAttack attackScript;
    private bool hasSpawnedRing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        transform.localScale = Vector3.zero;
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPlayerIDServerRpc(ulong id)
    {
        playerID.Value = id;
    }

    public void StartExpansion(Vector3 direction, UltimateAttack attack)
    {
        launchDirection = direction;
        attackScript = attack;
    }

    public void InitializeUltimate(Vector3 direction, CharacterClass.PlayerTeam team)
        {
            if (!IsServer) return; // Only the server should control initialization

            launchDirection = direction;
            playerTeam = team;
            StartExpansionServerRpc();
        }

    [ServerRpc]
    private void StartExpansionServerRpc()
    {
        StartExpansionClientRpc();
    }

    [ClientRpc]
    private void StartExpansionClientRpc()
    {
        StartCoroutine(ExpandCoroutine());
    }

    private IEnumerator ExpandCoroutine()
    {
        float startTime = Time.time;
        while (Time.time - startTime < expansionTime)
        {
            float progress = (Time.time - startTime) / expansionTime;
            float scaleFactor = Mathf.Lerp(0.1f, maxScale, progress);
            
            if (IsServer)
                currentScale.Value = scaleFactor; // Server updates the scale variable

            transform.localScale = Vector3.one * scaleFactor;
            yield return null;
        }

        hasExpanded = true;
        LaunchForward();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) 
        {
            transform.localScale = Vector3.one * currentScale.Value; // Sync scale for clients
        }
    }

    void LaunchForward()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = launchDirection * Speed;
        }
        Debug.Log("Ultimate launched!");
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || hasSpawnedRing) return;

        bool hitCharacter = false;
        CharacterClass character = collision.gameObject.GetComponent<CharacterClass>();
        
        if (character != null && character.getPlayersTeam() != playerTeam)
        {
            if (character.getPlayerID() != playerID.Value)
            {
                character.TakeDamage(damage);
                hitCharacter = true;   
            }
        }

        if (collision.gameObject.CompareTag("Ground") || hitCharacter)
        {
            Vector3 impactPosition = collision.contacts[0].point;
            SpawnRingServerRpc(impactPosition);
            hasSpawnedRing = true;
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject.IsSpawned)
            {
                networkObject.Despawn(true); // Ensures all clients properly unregister the object
            }
        }
    }

    [ServerRpc]
    private void SpawnRingServerRpc(Vector3 impactPosition)
    {
        GameObject ringToSpawn = null;
        Debug.Log("Spawning Ring at: " + impactPosition);
        if (playerTeam == CharacterClass.PlayerTeam.Fire && fireRingPrefab != null)
        {
            Debug.Log("Spawning Fire Ring!");
            ringToSpawn = Instantiate(fireRingPrefab, impactPosition, Quaternion.identity);
        }
        else if (playerTeam == CharacterClass.PlayerTeam.Water && waterRingPrefab != null)
        {
            Debug.Log("Spawning Water Ring!");
            ringToSpawn = Instantiate(waterRingPrefab, impactPosition, Quaternion.identity);
        }

        if (ringToSpawn != null)
        {
            NetworkObject ringNetworkObject = ringToSpawn.GetComponent<NetworkObject>();
            if (ringNetworkObject != null)
            {
                ringNetworkObject.Spawn(true);
            }
        }
    }

}
