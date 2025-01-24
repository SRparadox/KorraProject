using UnityEngine;

public class VitalStream: MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float projectileSpeed;

    public void Trigger()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {

            }

            Destroy(projectile, 5f);
        }
    }
}
