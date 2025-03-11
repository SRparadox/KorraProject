using UnityEngine;

public class WaterRingAttack: MonoBehaviour
{
    //public GameObject waterRingPrefab;
    //public GameObject fireRingPrefab;
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
            WaterRing waterring = waterRing.GetComponent<WaterRing>();

            if(waterring != null)
            {
                waterring.SetPlayer(GetComponent<CharacterClass>());
            }

            waterRing.tag = gameObject.tag;
            Debug.Log("Waterring spawned");
        } else
        {
            Debug.LogError("Water ring prefab not assigned in the Inspector");
        }
    }

}
