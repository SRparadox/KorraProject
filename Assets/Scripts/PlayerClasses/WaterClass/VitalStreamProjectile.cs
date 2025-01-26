using UnityEngine;

public class VitalStreamProjectile: MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("target hit");
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Damaged enemy!");

            // Destroy vital stream projectile on collison
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
