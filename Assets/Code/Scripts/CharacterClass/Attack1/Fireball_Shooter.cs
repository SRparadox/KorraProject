using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball_Shooter : MonoBehaviour
{
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    private Animator animator;
    
    private GameObject selectedPrefab; //stores which prefab the player will use

    private void Start()
    {
        //UpdateSelectedPrefab();
        animator = GetComponent<Animator>();
    }
    public void Trigger()
    {
        ShootFireball();
    }

    public void disableBuffer() {
        if (animator != null) animator.SetBool("BufferPunch", false);
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

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }
}
