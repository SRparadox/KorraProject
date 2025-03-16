using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerTracker : MonoBehaviour
{
    [NonSerialized] public GameObject instance;
    public CharacterClass.PlayerTeam playerTeam;
    public GameObject playerPrefab;
    [NonSerialized] public GameObject player;
    [NonSerialized] public CharacterClass playerClass;
    [NonSerialized] public bool isOneHanded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //Dont create two of these
        if (instance == null)
        {
            instance = this.gameObject;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnSceneLoad(GameManager gameManager)
    {
        player = gameManager.CreatePlayer(playerTeam);
        playerClass = player.GetComponent<CharacterClass>();
        GetPlayer().GetComponent<StarterAssets.StarterAssetsInputs>().setOneHanded(isOneHanded);
    }

    public void SetPlayerTeam(CharacterClass.PlayerTeam playerTeam)
    {
        this.playerTeam = playerTeam;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
    public CharacterClass getCharacterClass()
    {
        return playerClass;
    }

    public void toggleOneHandedMode()
    {
        isOneHanded = !isOneHanded;
    }

    public void setOneHanded(bool oneHanded)
    {
        isOneHanded = oneHanded;
        if (GetPlayer() == null) return;
        GetPlayer().GetComponent<StarterAssets.StarterAssetsInputs>().setOneHanded(oneHanded);
    }
}
