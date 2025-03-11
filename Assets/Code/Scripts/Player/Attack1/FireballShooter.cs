using UnityEngine;
using UnityEngine.InputSystem;

public class FireballShooter: MonoBehaviour
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

    public void disableBuffer()
    {
        if (animator != null)
            animator.SetBool("BufferPunch", false);
    }

    void ShootFireball()
    {
        if (selectedPrefab != null && fireballSpawnPoint != null)
        {
            GameObject Fireball = Instantiate(selectedPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
            Fireball fireball = Fireball.GetComponent<Fireball>();
            Camera mainCamera = Camera.main;

            if(fireball != null)
            {
                fireball.SetPlayer(GetComponent<CharacterClass>());
            }
            fireball.tag = gameObject.tag;

            if (mainCamera != null)
            {
                Vector3 cameraForward = mainCamera.transform.forward;
                Rigidbody rb = Fireball.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.linearVelocity = cameraForward * fireballSpeed;
                }
            }

            Destroy(Fireball, 3f);
        }
    }

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }
}
