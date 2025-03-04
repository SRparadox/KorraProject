using UnityEngine;
using UnityEngine.Android;

public class GameManager : MonoBehaviour
{
    public ZoneControl[] zones;
    public Transform Firespawn;
    public Transform Waterspawn;
    public float roundDuration = 300f;
    public int maxControlScore = 100;
    public int scoreIncrement = 5; // How much the score increases each tick
    public float scoreTickRate = 2f; //How often score increases
    public Material waterMaterial, lavaMaterial;
    public Renderer waterRenderer;
    public Material defaultSkybox, fireSkybox;

    private float roundTimer;
    private float scoreTimer = 0f;
    private ZoneControl activeZone;
    private string currentWinningTeam = "Neutral";
    private int fireScore = 0;
    private int waterScore = 0;
    private int fireWins = 0;
    private int waterWins = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChooseNewZone();
        SpawnPlayers();
        roundTimer = roundDuration;
    }

    void Update()
    {
        roundTimer -= Time.deltaTime;
        scoreTimer += Time.deltaTime;

        if(scoreTimer >= scoreTickRate)
        {
            scoreTimer = 0f;
            UpdateControlScore();
        }

        if (roundTimer <= 0 || fireScore >= maxControlScore || waterScore >= maxControlScore)
        {
            EndRound();
        }

        UpdateEnvironment();
    }

    void ChooseNewZone()
    {
        if (zones.Length == 0) return;

        activeZone = zones[Random.Range(0, zones.Length)];
        Debug.Log($"New active zone:{activeZone.gameObject.name}");
    }

    void UpdateControlScore()
    {
        if(activeZone == null) return;

        switch (activeZone.controllingTeam)
        {
            case "Fire":
                fireScore += scoreIncrement;
                break;
            case "Water":
                waterScore += scoreIncrement; 
                break;
        }
    }

    void UpdateEnvironment()
    {
        string dominantTeam = fireScore > waterScore ? "Fire" : waterScore > fireScore ? "Water" : "Neutral";

        if (currentWinningTeam == dominantTeam) return;
        currentWinningTeam = dominantTeam;

        switch(dominantTeam)
        {
            case "Fire":
                waterRenderer.material = lavaMaterial;
                RenderSettings.skybox = fireSkybox;
                break;
            case "Water":
                waterRenderer.material = waterMaterial;
                RenderSettings.skybox = defaultSkybox;
                break;
            case "Neutral":
                waterRenderer.material = waterMaterial;
                RenderSettings.skybox = defaultSkybox;
                break;

        }

        DynamicGI.UpdateEnvironment();
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

    void EndRound()
    {
        string roundWinner = fireScore > waterScore ? "Fire" : "Water";

        if(roundWinner == "Fire")
        {
            fireWins++;
            Debug.Log("Fire won the round");
            waterWins = 0;
        }
        else
        {
            waterWins++;
            Debug.Log("Water won the round");
            fireWins = 0;
        }

        if(fireWins == 2)
        {
            Debug.Log("Fire Team Wins");
            ResetGame(); //Reset Game to beginning state
            return;
        }
        else if(waterWins == 2)
        {
            Debug.Log("Water Team Wins");
            ResetGame(); //Reset Game to beginning state
            return;
        }

        ResetRound(); //Set up next round
    }

    void ResetRound()
    {
        fireScore = 0;
        waterScore = 0;
        ChooseNewZone();
        SpawnPlayers();
        roundTimer = roundDuration;
    }

    void ResetGame()
    {
        fireWins = 0;
        waterWins = 0;
        ResetRound();
    }
}
