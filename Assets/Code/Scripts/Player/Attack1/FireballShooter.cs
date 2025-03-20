using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballShooter: NetworkBehaviour
{
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    private Animator animator;

    private GameObject selectedPrefab; //stores which prefab the player will use
    [SerializeField] private GameObject fireballPrefab, waterballPrefab; //the prefab that will be used to shoot


    private void Start()
    {
        //UpdateSelectedPrefab();
        animator = GetComponent<Animator>();
    }
    public void Trigger()
    {
        if (!IsOwner)
            return;
        ShootFireball();
    }

    public void disableBuffer()
    {
        if (animator != null)
            animator.SetBool("BufferPunch", false);
    }

    void ShootFireball()
    {
        if (selectedPrefab != null && fireballSpawnPoint != null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 spawnPos = fireballSpawnPoint.position;
            Quaternion spawnRot = fireballSpawnPoint.rotation;
            Vector3 shootDirection = mainCamera.transform.forward;

            if (IsHost) // If the player is the host, spawn the fireball locally
            {
                spawnFireballForServerOwner(spawnPos, spawnRot, shootDirection);
            }
            else // Request the server to spawn the fireball
                ShootFireballServerRpc(spawnPos, spawnRot, shootDirection, NetworkManager.Singleton.LocalClientId);
        }
    }

    private void spawnFireballForServerOwner(Vector3 spawnPos, Quaternion spawnRot, Vector3 shootDirection)
    {
        if (selectedPrefab == null){
            if (GetComponent<CharacterClass>().team.Value == CharacterClass.PlayerTeam.Fire){
                selectedPrefab = fireballPrefab;
            } else {
                selectedPrefab = waterballPrefab;
            }
        }
        GameObject Fireball = Instantiate(selectedPrefab, spawnPos, spawnRot);
        Fireball fireball = Fireball.GetComponent<Fireball>();

        Rigidbody rb = Fireball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Fireball is missing a Rigidbody component!");
            return;
        }

        NetworkObject netObj = Fireball.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("Fireball is missing a NetworkObject component!");
            return;
        }

        rb.useGravity = false;
        rb.linearVelocity = shootDirection * fireballSpeed;

        netObj.Spawn();
        fireball.setPlayer(GetComponent<CharacterClass>(), NetworkManager.Singleton.LocalClientId);
        Destroy(Fireball, 3f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootFireballServerRpc(Vector3 spawnPos, Quaternion spawnRot, Vector3 shootDirection, ulong shooterId)
    {
        if (!IsServer) return;
        if (shooterId == 0) {
            Debug.LogError("Fireball shooter ID is invalid!");
            return;
        }
        if (selectedPrefab == null){
            if (GetComponent<CharacterClass>().team.Value == CharacterClass.PlayerTeam.Fire){
                selectedPrefab = fireballPrefab;
            } else {
                selectedPrefab = waterballPrefab;
            }
        }
        GameObject Fireball = Instantiate(selectedPrefab, spawnPos, spawnRot);
        Fireball fireball = Fireball.GetComponent<Fireball>();

        Rigidbody rb = Fireball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Fireball is missing a Rigidbody component!");
            return;
        }

        NetworkObject netObj = Fireball.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("Fireball is missing a NetworkObject component!");
            return;
        }

        rb.useGravity = false;
        rb.linearVelocity = shootDirection * fireballSpeed;

        netObj.Spawn();
        fireball.SetPlayerServerRpc(shooterId);
        Destroy(Fireball, 3f);
    }

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }
}
