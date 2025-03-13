using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballShooter : NetworkBehaviour
{
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    private Animator animator;
    private GameObject selectedPrefab; // Which prefab the player will use

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Trigger()
    {
        if (IsOwner)
            ShootFireballServerRpc();
    }

    [ServerRpc]
    private void ShootFireballServerRpc()
    {
        ShootFireball();
    }

    void ShootFireball()
    {
        if (selectedPrefab != null && fireballSpawnPoint != null)
        {
            GameObject fireballObj = Instantiate(selectedPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
            var netObj = fireballObj.GetComponent<NetworkObject>();
            if (netObj != null)
                netObj.Spawn();
            Fireball fireball = fireballObj.GetComponent<Fireball>();
            Camera mainCamera = Camera.main;
            if (fireball != null)
                fireball.SetPlayer(GetComponent<CharacterClass>());
            if (mainCamera != null)
            {
                Vector3 cameraForward = mainCamera.transform.forward;
                Rigidbody rb = fireballObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.linearVelocity = cameraForward * fireballSpeed;
                }
            }
            Destroy(fireballObj, 3f);
        }
    }

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }
}
