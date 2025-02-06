using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball_Shooter : MonoBehaviour
{
    public GameObject fireball_Prefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    

    public void Trigger()
    {
        ShootFireball();
    }

    void ShootFireball()
    {
        if (fireball_Prefab != null && fireballSpawnPoint != null)
        {
            GameObject fireball = Instantiate(fireball_Prefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);

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
}
