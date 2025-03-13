using Unity.Netcode;
using UnityEngine;

public class Ultimate : NetworkBehaviour
{
    public GameObject fireRingPrefab;
    public GameObject waterRingPrefab;
    public float expansionTime = 5f;
    public float maxScale = 8f;
    public float Speed = 20f;
    public float lifeTime = 5f;
    public float damage = 75f;

    private CharacterClass.PlayerTeam playerTeam;
    private float currentTime = 0f;
    private bool hasExpanded = false;
    private Rigidbody rb;
    private Vector3 launchDirection;
    private UltimateAttack attackScript;
    private bool hasSpawnedRing = false;

    public void Initialize(CharacterClass.PlayerTeam team)
    {
        playerTeam = team;
        Debug.Log("Ultimate initialized for " + playerTeam);
    }
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
    public void StartExpansion(Vector3 direction, UltimateAttack attack)
    {
        launchDirection = direction;
        attackScript = attack;
    }
    void Update()
    {
        if (!IsServer) return;
        if (!hasExpanded)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentTime / expansionTime);
            float scaleFactor = Mathf.Lerp(0.1f, maxScale, progress);
            transform.localScale = Vector3.one * scaleFactor;
            if (progress >= 1f)
            {
                hasExpanded = true;
                LaunchForward();
            }
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
        Debug.Log("Ultimate launched! Resetting camera...");
        attackScript.ResetCamera();
        Destroy(gameObject, lifeTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || hasSpawnedRing) return;
        bool hitCharacter = false;
        CharacterClass target = collision.gameObject.GetComponent<CharacterClass>();
        if (target != null && target.getPlayersTeam() != playerTeam)
        {
            target.TakeDamage(damage);
            hitCharacter = true;
        }
        if (collision.gameObject.CompareTag("Ground") || hitCharacter)
        {
            Vector3 impactPosition = collision.contacts[0].point;
            GameObject ringToSpawn = null;
            if (playerTeam == CharacterClass.PlayerTeam.Fire && fireRingPrefab != null)
                ringToSpawn = fireRingPrefab;
            else if (playerTeam == CharacterClass.PlayerTeam.Water && waterRingPrefab != null)
                ringToSpawn = waterRingPrefab;
            if (ringToSpawn != null)
            {
                GameObject ringObj = Instantiate(ringToSpawn, impactPosition, Quaternion.identity);
                var netObj = ringObj.GetComponent<NetworkObject>();
                if (netObj != null)
                    netObj.Spawn();
                hasSpawnedRing = true;
            }
            else
            {
                Debug.LogError("No valid ring prefab assigned for " + playerTeam);
            }
            Destroy(gameObject);
        }
    }
}
