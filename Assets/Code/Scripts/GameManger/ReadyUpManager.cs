using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReadyUpManager: NetworkBehaviour
{
    public TMP_Text countdownText;
    public GameObject readyMenu;
    public Button readyButton;
    public GameObject UIMenu;
    public GameObject matchText;
    public GameObject settingsMenu;

    private Dictionary<ulong, bool> playerReadyStatus = new Dictionary<ulong, bool>();
    private List<GameObject> players = new List<GameObject>();
    private bool allReady = false;
    private int countdown = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is missing! Make sure it exists in the scene.");
            return;
        }

        Debug.Log("ReadyUpManager started!");
        if (IsServer)
        {
            FindPlayers();
        }

        readyButton.onClick.AddListener(PlayerReady);
        UIMenu.SetActive(false);
        matchText.gameObject.SetActive(false);
        ShowCursor();

    }

    void FindPlayers()
    {
        players.Clear();
        playerReadyStatus.Clear();

        GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in existingPlayers)
        {
            NetworkObject netObj = player.GetComponent<NetworkObject>();

            if (netObj != null && netObj.IsSpawned)
            {
                ulong clientId = netObj.OwnerClientId;
                players.Add(player);
                playerReadyStatus[clientId] = false;
                player.GetComponent<PlayerInput>().enabled = false;
            }
        }
    }

    public void PlayerReady()
    {
        Debug.Log("Ready button clicked!");
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        if (playerReadyStatus.ContainsKey(clientId) && !playerReadyStatus[clientId])
        {
            SetPlayerReadyServerRpc(clientId);
            readyButton.gameObject.SetActive(false);
            matchText.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerReadyServerRpc(ulong clientId)
    {
        Debug.Log($"SetPlayerReadyServerRpc called for client {clientId}");
        if (playerReadyStatus.ContainsKey(clientId))
        {
            playerReadyStatus[clientId] = true;
        }

        if (AllPlayersReady())
        {
            allReady = true;
            ShowMatchTextClientRpc(false);
            StartCoroutine(StartMatchCountdown());
        }
    }

    bool AllPlayersReady()
    {
        foreach (var ready in playerReadyStatus.Values)
        {
            if (!ready)
                return false;
        }
        return true;
    }

    IEnumerator StartMatchCountdown()
    {
        ShowMatchTextClientRpc(true);

        readyMenu.SetActive(true);
        while (countdown > 0)
        {
            UpdateCountdownTextClientRpc(countdown);
            yield return new WaitForSeconds(1);
            countdown--;
        }

        readyMenu.SetActive(false);
        UIMenu.SetActive(true);
        StartMatchClientRpc();
    }

    [ClientRpc]
    void UpdateCountdownTextClientRpc(int conut)
    {
        countdownText.text = conut.ToString();
    }

    [ClientRpc]
    void ShowMatchTextClientRpc(bool show)
    {
        readyMenu.SetActive(show);
    }

    [ClientRpc]
    void StartMatchClientRpc()
    {
        foreach (var player in players)
        {
            player.GetComponent<PlayerInput>().enabled = true;
        }
        HideCursor();
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HideCursor()
    {
        if (settingsMenu != null && settingsMenu.activeSelf)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ResetReadyUp()
    {
        FindPlayers();

        foreach (GameObject player in players)
        {
            ulong clientId = player.GetComponent<NetworkObject>().OwnerClientId;
            playerReadyStatus[clientId] = false;
        }

        readyButton.gameObject.SetActive(true);
        matchText.SetActive(false);
        UIMenu.SetActive(false);
        readyMenu.SetActive(true);

        countdown = 5;

        ShowCursor();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
