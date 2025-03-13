using UnityEngine;
using Unity.Netcode;

public class WaterRingAttack : MonoBehaviour
{
    private GameObject selectedPrefab;

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

    public void Trigger()
    {
        SpawnWaterRing();
    }

    private void SpawnWaterRing()
    {
        if (selectedPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 1, 0);
            GameObject waterRing = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            var netObj = waterRing.GetComponent<NetworkObject>();
            if (netObj != null)
                netObj.Spawn();
            WaterRing waterRingScript = waterRing.GetComponent<WaterRing>();
            if (waterRingScript != null)
                waterRingScript.SetPlayer(GetComponent<CharacterClass>());
            Debug.Log("Water ring spawned");
        }
        else
        {
            Debug.LogError("Water ring prefab not assigned in the Inspector");
        }
    }
}
