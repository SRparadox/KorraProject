using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ZoneControl[] zones;
    public Transform Firespawn;
    public Transform Waterspawn;

    private ZoneControl activeZone;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChooseNewZone();
        SpawnPlayers();
    }

    void ChooseNewZone()
    {
        if (zones.Length == 0) return;

        activeZone = zones[Random.Range(0, zones.Length)];
        Debug.Log($"New active zone:{activeZone.gameObject.name}");
    }

    void SpawnPlayers()
    {
        GameObject[] fireplayers = GameObject.FindGameObjectsWithTag("Fire");
        GameObject[] waterplayers = GameObject.FindGameObjectsWithTag("Water");

        SpawnTeam(fireplayers, Firespawn);
        SpawnTeam(waterplayers, Waterspawn);
    }

   void SpawnTeam(GameObject[] players, Transform spawnPoint)
   {
        if(spawnPoint == null) return;

        foreach(GameObject player in players)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }
   }

   public void RespawnPlayer(GameObject player)
   {
        if (player.CompareTag("Fire"))
        {
            player.transform.position = Firespawn.position;
            player.transform.rotation = Firespawn.rotation;
        }
        else if (player.CompareTag("Water"))
        {
            player.transform.position = Waterspawn.position;
            player.transform.rotation = Waterspawn.rotation;
        }

        Debug.Log($"{player.name} respawned at {player.transform.position}");
   }
}
