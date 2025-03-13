using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public ZoneControl[] zones;
    public Transform Firespawn;
    public Transform Waterspawn;
    public float roundDuration = 300f;
    public int maxControlScore = 100;
    public int scoreIncrement = 5; // How much the score increases each tick
    public float scoreTickRate = 2f; // How often score increases
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
    public GameObject PowerUpSpawnParent;
    public float powerUpSpawnInterval = 8f;
    public int initialPowers = 3;
    public GameObject playerPrefab;

    // Synchronized state via NetworkVariables (server writes, everyone reads)
    private NetworkVariable<float> roundTimer = new NetworkVariable<float>();
    private float scoreTimer = 0f;
    private ZoneControl activeZone;
    private string currentWinningTeam = "Neutral";
    private NetworkVariable<int> fireScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> waterScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private PlayerTracker localPlayer;
    [SerializeField] private GameObject playersParent;
    private NetworkVariable<int> fireWins = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> waterWins = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            roundTimer.Value = roundDuration;
            StartCoroutine(spawnPowerUpEveryXSeconds(powerUpSpawnInterval));
        }
    }

    void Start()
    {
        GameObject localPlayerInput = GameObject.Find("KeepTrackOfPlayer");
        if (localPlayerInput != null)
        {
            localPlayer = localPlayerInput.GetComponent<PlayerTracker>();
            Debug.Log("Deleting Players...");
            DeleteAllPlayers();
            localPlayer.OnSceneLoad(this);
        }
        else
        {
            Debug.LogError("PlayerTracker not found");
        }

        ChooseNewZone();
        SpawnPlayers();

        if (IsServer)
        {
            roundTimer.Value = roundDuration;
            for (int i = 0; i < initialPowers; i++)
            {
                spawnAPowerup();
            }
        }
    }

    IEnumerator spawnPowerUpEveryXSeconds(float seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            if (IsServer)
                spawnAPowerup();
        }
    }

    void spawnAPowerup()
    {
        // Get all children under PowerUpSpawnParent
        Transform parentTransform = PowerUpSpawnParent.transform;
        int childCount = parentTransform.childCount;
        if(childCount == 0)
        {
            Debug.LogError("No children under PowerUpSpawnParent");
            return;
        }
        Transform[] children = new Transform[childCount];
        bool allAreActive = true;
        for (int i = 0; i < childCount; i++)
        {
            children[i] = parentTransform.GetChild(i);
            if (!children[i].GetComponent<PowerUpGiver>().isActive)
                allAreActive = false;
        }
        if (children.Length == 0 || allAreActive)
        {
            Debug.LogError("Powerups: No children found or all are active.");
            return;
        }
        int randomIndex = Random.Range(0, children.Length);
        Transform randomChild = children[randomIndex];
        PowerUpGiver powerUpGiver = randomChild.GetComponent<PowerUpGiver>();
        while (powerUpGiver.isActive)
        {
            randomIndex = Random.Range(0, children.Length);
            randomChild = children[randomIndex];
            powerUpGiver = randomChild.GetComponent<PowerUpGiver>();
        }
        if (powerUpGiver != null)
        {
            powerUpGiver.spawnPowerUp();
        }
    }

    void Update()
    {
        if (!IsServer) return; // Only the server updates game state

        roundTimer.Value -= Time.deltaTime;
        scoreTimer += Time.deltaTime;
        updateTimeUI();
        UpdateProgessBars();

        if (scoreTimer >= scoreTickRate)
        {
            scoreTimer = 0f;
            UpdateControlScore();
        }

        if (roundTimer.Value <= 0 || fireScore.Value >= maxControlScore || waterScore.Value >= maxControlScore)
        {
            EndRound();
        }

        UpdateEnvironment();
    }

    void updateTimeUI()
    {
        int minutes = Mathf.FloorToInt(roundTimer.Value / 60);
        int seconds = Mathf.FloorToInt(roundTimer.Value % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void ChooseNewZone()
    {
        if (zones.Length == 0) return;
        if (activeZone != null && activeZone.lightPillar != null)
        {
            activeZone.lightPillar.SetActive(false);
        }
        activeZone = zones[Random.Range(0, zones.Length)];
        if (activeZone.lightPillar != null)
        {
            activeZone.lightPillar.SetActive(true);
        }
        Debug.Log($"New active zone: {activeZone.gameObject.name}");
    }

    void UpdateControlScore()
    {
        if (activeZone == null) return;
        switch (activeZone.controllingTeam)
        {
            case "Fire":
                fireScore.Value = Mathf.Min(fireScore.Value + scoreIncrement, maxControlScore);
                break;
            case "Water":
                waterScore.Value = Mathf.Min(waterScore.Value + scoreIncrement, maxControlScore);
                break;
            case "Neutral":
                return;
        }
    }

    void UpdateProgessBars()
    {
        float smoothSpeed = 5f * Time.deltaTime;
        fireProgressBar.value = Mathf.Lerp(fireProgressBar.value, (float)fireScore.Value / maxControlScore, smoothSpeed);
        waterProgressBar.value = Mathf.Lerp(waterProgressBar.value, (float)waterScore.Value / maxControlScore, smoothSpeed);
    }

    void UpdateWinIcons(Image[] teamIcons, int wins, Sprite[] teamSprites)
    {
        for (int i = 0; i < teamIcons.Length; i++)
        {
            teamIcons[i].sprite = (i < wins) ? teamSprites[1] : teamSprites[0];
        }
    }

    void UpdateEnvironment()
    {
        string dominantTeam = fireScore.Value > waterScore.Value ? "Fire" : waterScore.Value > fireScore.Value ? "Water" : "Neutral";
        if (currentWinningTeam == dominantTeam) return;
        currentWinningTeam = dominantTeam;
        switch (dominantTeam)
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

    public GameObject CreatePlayer(CharacterClass.PlayerTeam team)
    {
        if (!IsServer) return null;
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, playersParent.transform);
        player.GetComponent<CharacterClass>().setPlayersTeam(team);
        RespawnPlayer(player);
        var netObj = player.GetComponent<NetworkObject>();
        if (netObj != null)
            netObj.Spawn();
        return player;
    }

    void SpawnPlayers()
    {
        List<GameObject> fireplayers = new List<GameObject>();
        List<GameObject> waterplayers = new List<GameObject>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<CharacterClass>().getPlayersTeam() == CharacterClass.PlayerTeam.Fire)
                fireplayers.Add(player);
            else if (player.GetComponent<CharacterClass>().getPlayersTeam() == CharacterClass.PlayerTeam.Water)
                waterplayers.Add(player);
        }
        SpawnTeam(fireplayers.ToArray(), Firespawn);
        SpawnTeam(waterplayers.ToArray(), Waterspawn);
    }

    void SpawnTeam(GameObject[] players, Transform spawnPoint)
    {
        if (spawnPoint == null) return;
        foreach (GameObject player in players)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;
        string teamName = "None";
        var charClass = player.GetComponent<CharacterClass>();
        if (charClass.getPlayersTeam() == CharacterClass.PlayerTeam.Fire)
        {
            teamName = "Fire";
            player.transform.position = Firespawn.position;
            player.transform.rotation = Firespawn.rotation;
        }
        else if (charClass.getPlayersTeam() == CharacterClass.PlayerTeam.Water)
        {
            teamName = "Water";
            player.transform.position = Waterspawn.position;
            player.transform.rotation = Waterspawn.rotation;
        }
        else
            Debug.Log("Player team not set");
        if (controller != null)
            controller.enabled = true;
        Debug.Log($"{player.name}/{teamName} respawned at {player.transform.position}");
    }

    void EndRound()
    {
        string roundWinner = fireScore.Value > waterScore.Value ? "Fire" : "Water";
        if (roundWinner == "Fire")
        {
            fireWins.Value++;
            UpdateWinIcons(fireWinIcons, fireWins.Value, fireWinSprites);
        }
        else
        {
            waterWins.Value++;
            UpdateWinIcons(waterWinIcons, waterWins.Value, waterWinSprites);
        }
        if (fireWins.Value == 2 || waterWins.Value == 2)
        {
            Debug.Log(roundWinner + " Team Wins");
            ResetGame();
            return;
        }
        ResetRound();
    }

    void ResetRound()
    {
        fireScore.Value = 0;
        waterScore.Value = 0;
        fireProgressBar.value = 0;
        waterProgressBar.value = 0;
        if (activeZone != null)
            activeZone.controllingTeam = "Neutral";
        ChooseNewZone();
        SpawnPlayers();
        roundTimer.Value = roundDuration;
    }

    void ResetWinIcons(Image[] teamIcons, Sprite[] teamSprites)
    {
        for (int i = 0; i < teamIcons.Length; i++)
        {
            teamIcons[i].sprite = teamSprites[0];
        }
    }

    void DeleteAllPlayers()
    {
        foreach (Transform child in playersParent.transform)
        {
            if (child.tag == "Player")
            {
                Destroy(child.gameObject);
            }
        }
    }

    void ResetGame()
    {
        fireWins.Value = 0;
        waterWins.Value = 0;
        ResetRound();
        if (activeZone != null)
            activeZone.controllingTeam = "Neutral";
        ResetWinIcons(fireWinIcons, fireWinSprites);
        ResetWinIcons(waterWinIcons, waterWinSprites);
    }
}
