using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZoneControl : MonoBehaviour
{
    private HashSet<GameObject> firePlayers = new HashSet<GameObject>();
    private HashSet<GameObject> waterPlayers = new HashSet<GameObject>();

    public string controllingTeam = "Neutral";
    public GameObject lightPillar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterClass>() == null)
        {
            return;
        }
        if (other.gameObject.GetComponent<CharacterClass>().getPlayersTeam() == CharacterClass.PlayerTeam.Fire)
        {
            firePlayers.Add(other.gameObject);
        }
        else if (other.gameObject.GetComponent<CharacterClass>().getPlayersTeam() == CharacterClass.PlayerTeam.Water)
        {
            waterPlayers.Add(other.gameObject);
        }

        UpdateControl();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterClass>().getPlayersTeam() == CharacterClass.PlayerTeam.Fire)
        {
            firePlayers.Remove(other.gameObject);
        }
        else if (other.gameObject.GetComponent<CharacterClass>().getPlayersTeam() == CharacterClass.PlayerTeam.Water)
        {
            waterPlayers.Remove(other.gameObject);
        }

        UpdateControl();
    }

    // Update is called once per frame
    void UpdateControl()
    {
        if(firePlayers.Count > waterPlayers.Count)
        {
            controllingTeam = "Fire";
        }
        else if(waterPlayers.Count > firePlayers.Count)
        {
            controllingTeam = "Water";
        }
        else
        {
            controllingTeam = "Neutral";
        }

        Debug.Log($"{gameObject.name} controlled by {controllingTeam}");
    }
}
