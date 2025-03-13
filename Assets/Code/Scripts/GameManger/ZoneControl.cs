using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ZoneControl : NetworkBehaviour
{
    private HashSet<GameObject> firePlayers = new HashSet<GameObject>();
    private HashSet<GameObject> waterPlayers = new HashSet<GameObject>();

    public string controllingTeam = "Neutral";
    public GameObject lightPillar;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        CharacterClass charClass = other.gameObject.GetComponent<CharacterClass>();
        if (charClass == null) return;
        if (charClass.getPlayersTeam() == CharacterClass.PlayerTeam.Fire)
            firePlayers.Add(other.gameObject);
        else if (charClass.getPlayersTeam() == CharacterClass.PlayerTeam.Water)
            waterPlayers.Add(other.gameObject);
        UpdateControl();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        CharacterClass charClass = other.gameObject.GetComponent<CharacterClass>();
        if (charClass == null) return;
        if (charClass.getPlayersTeam() == CharacterClass.PlayerTeam.Fire)
            firePlayers.Remove(other.gameObject);
        else if (charClass.getPlayersTeam() == CharacterClass.PlayerTeam.Water)
            waterPlayers.Remove(other.gameObject);
        UpdateControl();
    }

    void UpdateControl()
    {
        if (firePlayers.Count > waterPlayers.Count)
            controllingTeam = "Fire";
        else if (waterPlayers.Count > firePlayers.Count)
            controllingTeam = "Water";
        else
            controllingTeam = "Neutral";

        Debug.Log($"{gameObject.name} controlled by {controllingTeam}");
    }
}
