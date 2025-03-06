using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public Material defaultSkybox, fireSkybox, waterSkybox;
    public Slider fireProgressBar;
    public Slider waterProgressBar;
    public Image[] fireWinIcons;
    public Image[] waterWinIcons;
    public Sprite[] fireWinSprites;
    public Sprite[] waterWinSprites;
    public TMP_Text timerText;

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
        updateTimeUI();
        UpdateProgessBars();

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

    void updateTimeUI()
    {
        int minutes = Mathf.FloorToInt(roundTimer / 60);
        int seconds = Mathf.FloorToInt(roundTimer % 60);
        timerText.text = $"{minutes: 00}:{seconds:00}";
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
                fireScore = Mathf.Min(fireScore + scoreIncrement, maxControlScore);
                break;
            case "Water":
                waterScore = Mathf.Min(waterScore + scoreIncrement, maxControlScore); 
                break;
            case "Neutral":
                return;
        }

    }

    void UpdateProgessBars()
    {
        float smoothSpeed = 5f * Time.deltaTime;
        fireProgressBar.value = Mathf.Lerp(fireProgressBar.value, (float)fireScore / maxControlScore, smoothSpeed);
        waterProgressBar.value = Mathf.Lerp(waterProgressBar.value, (float)waterScore / maxControlScore, smoothSpeed);
    }

    void UpdateWinIcons(Image[] teamIcons, int wins, Sprite[] teamSprites)
    {
        for(int i = 0; i < teamIcons.Length; i++)
        {
            teamIcons[i].sprite = (i < wins) ? teamSprites[1] : teamSprites[0];
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
                RenderSettings.skybox = waterSkybox;
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
            UpdateWinIcons(fireWinIcons, fireWins, fireWinSprites);
        }
        else
        {
            waterWins++;
            UpdateWinIcons(waterWinIcons, waterWins, waterWinSprites);
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
        fireProgressBar.value = 0;
        waterProgressBar.value = 0;
        activeZone.controllingTeam = "Neutral";
        ChooseNewZone();
        SpawnPlayers();
        roundTimer = roundDuration;
    }

    void ResetWinIcons(Image[] teamIcons, Sprite[] teamSprites)
    {
        for (int i = 0; i < teamIcons.Length; i++)
        {
            teamIcons[i].sprite = teamSprites[0];
        }
    }

    void ResetGame()
    {
        fireWins = 0;
        waterWins = 0;
        ResetRound();
        activeZone.controllingTeam = "Neutral";
        ResetWinIcons(fireWinIcons, fireWinSprites);
        ResetWinIcons(waterWinIcons, waterWinSprites);
    }
}
