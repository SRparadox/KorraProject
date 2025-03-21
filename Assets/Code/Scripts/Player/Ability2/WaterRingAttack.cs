using Unity.Netcode;
using UnityEngine;

public class WaterRingAttack: NetworkBehaviour
{
    //public GameObject waterRingPrefab;
    //public GameObject fireRingPrefab;
    private GameObject selectedPrefab;
    ulong ringID = 0;

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

    public void Trigger()
    {
        if (!IsOwner) return; // Only the owner can spawn the water ring
        if (selectedPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 1, 0);
            SpawnWaterRingServerRpc(spawnPosition);
            GameObject waterRing = NetworkManager.Singleton.SpawnManager.SpawnedObjects[ringID].gameObject;
            if (waterRing == null) return;
            waterRing.GetComponent<WaterRing>().setPlayerIDServerRpc(NetworkManager.Singleton.LocalClientId);
            waterRing.GetComponent<WaterRing>().SetPlayer(GetComponent<CharacterClass>());
        }
    }

    [ServerRpc]
    private void SpawnWaterRingServerRpc(Vector3 spawnPosition)
    {
        GameObject waterRing = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        waterRing.GetComponent<NetworkObject>().Spawn(true);
        ringID = waterRing.GetComponent<NetworkObject>().NetworkObjectId;
    }



}
