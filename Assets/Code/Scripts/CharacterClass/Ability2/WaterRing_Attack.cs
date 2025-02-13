using UnityEngine;

public class WaterRing_Attack : MonoBehaviour
{
    public GameObject waterRingPrefab;
    public GameObject fireRingPrefab;

    public void Trigger()
    {
        SpawnWaterRing();
    }

    private void SpawnWaterRing()
    {
        GameObject selectPrefab = null;

        if (gameObject.CompareTag("Fire"))
        {
            selectPrefab = fireRingPrefab;
        }
        else if (gameObject.CompareTag("Water"))
        {
            selectPrefab = waterRingPrefab;
        }

        if(selectPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 1, 0);
            GameObject waterRing = Instantiate(selectPrefab, spawnPosition, Quaternion.identity);
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            Debug.Log("Waterring spawned");
        }
        else
        {
            Debug.LogError("Water ring prefab not assigned in the Inspector");
        }
    }
}
