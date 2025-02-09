using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball_Shooter : MonoBehaviour
{
    public GameObject fireball_Prefab;
    public GameObject waterball_Prefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    
    private GameObject selectedPrefab; //stores which prefab the player will use

    private void Start()
    {
        UpdateSelectedPrefab();
    }
    public void Trigger()
    {
        ShootFireball();
    }

    void ShootFireball()
    {
        if (selectedPrefab != null && fireballSpawnPoint != null)
        {
            GameObject fireball = Instantiate(selectedPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 cameraForward = mainCamera.transform.forward;
                Rigidbody rb = fireball.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.linearVelocity = cameraForward * fireballSpeed;
                }
            }

            Destroy(fireball, 3f);
        }
    }

    public void UpdateSelectedPrefab()
    {
        if (CompareTag("Fire"))
        {
            selectedPrefab = fireball_Prefab;
        }
        else if (CompareTag("Water"))
        {
            selectedPrefab = waterball_Prefab;
        }
        else
        {
            Debug.LogWarning("Player has no vaild element tag");
            selectedPrefab = fireball_Prefab;
        }
    }
}
