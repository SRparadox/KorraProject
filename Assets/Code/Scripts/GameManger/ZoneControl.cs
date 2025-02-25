using System.Collections.Generic;
using UnityEngine;

public class ZoneControl : MonoBehaviour
{
    private HashSet<GameObject> firePlayers = new HashSet<GameObject>();
    private HashSet<GameObject> waterPlayers = new HashSet<GameObject>();

    public string controllingTeam = "Neutral";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            firePlayers.Add(other.gameObject);
        }
        else if (other.CompareTag("Water"))
        {
            waterPlayers.Add(other.gameObject);
        }

        UpdateControl();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            firePlayers.Remove(other.gameObject);
        }
        else if (other.CompareTag("Water"))
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
