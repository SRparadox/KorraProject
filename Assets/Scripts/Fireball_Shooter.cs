using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball_Shooter : MonoBehaviour
{
    public GameObject fireball_Prefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    public PlayerInput input;

    private InputAction attackAction;

    void Awake()
    {
        attackAction = input.actions["Attack"];
        if (attackAction == null )
        {
            Debug.LogError("Attack action not found!");
        }
    }

    void Update()
    {
        if( attackAction != null && attackAction.triggered)
        {
            ShootFireball();
        }
    }

    void ShootFireball()
    {
        if( fireball_Prefab != null && fireballSpawnPoint != null)
        {
            GameObject fireball = Instantiate(fireball_Prefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);

            Rigidbody rb = fireball.GetComponent<Rigidbody>();
            if( rb != null )
            {
                rb.useGravity = false;
                rb.linearVelocity = fireballSpawnPoint.transform.forward * fireballSpeed;
            }

            Destroy( fireball, 3f );
        } 
    }
}
