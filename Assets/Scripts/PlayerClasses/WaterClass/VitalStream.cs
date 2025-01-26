using System.ComponentModel;
using UnityEngine;

public class VitalStream: MonoBehaviour
{
    [Category("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileOrigin;

    [Category("Attack Properties")]
    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private float damageAmount = 10f;

    [SerializeField] private Camera playerCamera;

    public void Trigger()
    {
        if (projectilePrefab != null && projectileOrigin != null)
        {
            Quaternion spawnRotation = projectileOrigin.rotation;
            spawnRotation *= Quaternion.Euler(90f, 0f, 0f);

            GameObject projectile = Instantiate(projectilePrefab, projectileOrigin.position, spawnRotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 cameraForward = playerCamera.transform.forward;
                cameraForward.y = 0;

                rb.linearVelocity = cameraForward.normalized * projectileSpeed;
            }

            Destroy(projectile, 3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Damaged enemy!");

            // Destroy vital stream projectile on collison
            Destroy(other.gameObject);
        }
    }
}
