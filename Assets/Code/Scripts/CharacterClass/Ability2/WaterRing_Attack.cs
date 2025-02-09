using UnityEngine;

public class WaterRing_Attack : MonoBehaviour
{
    public GameObject waterRingPrefab;

    public void Trigger()
    {
        SpawnWaterRing();
    }

    private void SpawnWaterRing()
    {
        if(waterRingPrefab != null)
        {
            GameObject waterRing = Instantiate(waterRingPrefab, transform.position, Quaternion.identity);
            Debug.Log("Waterring spawned");
        }
        else
        {
            Debug.LogError("Water ring prefab not assigned in the Inspector");
        }
    }
}
