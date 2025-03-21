using UnityEngine;
using Unity.Netcode;
using UnityEditor.Rendering;
using System.Collections;
using UnityEngine.Analytics;

public class Fireball : NetworkBehaviour
{
    public int damage = 10;
    private CharacterClass player;

    // Store the shooter's NetworkObject ID
    private NetworkVariable<ulong> shooterID = new NetworkVariable<ulong>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );
    private NetworkVariable<int> team = new NetworkVariable<int>(0); // Default team value

    private void Start()
    {
        // Start the coroutine to delete the fireball after a certain time
        StartCoroutine(deleteFireballAfterTime(4f));
    }

    [ServerRpc(RequireOwnership = false)]
    public void setTeamServerRpc(int team)
    {
        this.team.Value = team;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only process collision on the client that owns the hit player
        CharacterClass target = collision.gameObject.GetComponent<CharacterClass>();
        NetworkObject targetNetworkObject = collision.gameObject.GetComponent<NetworkObject>();

        if (target == null) 
        {
            deleteFireballAfterTime(0.5f); //Lag compensation for fireball
            return;
        }

        if (target.IsHost) {
        }
        else if (!target.IsOwner) return; // Only the player's own client should handle health

        if (player == null)
        {
            if (shooterID.Value == 0) //Server ID is owner
            {
                //Set player to owner of network
                player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[NetworkManager.Singleton.LocalClientId].GetComponent<CharacterClass>();
            }

            if (player == null)
            {
                AssignPlayer();
            }
            if (player == null && team.Value == 0)
            {
            
                Debug.LogError("Fireball's shooter is missing and team not set!");
                deleteFireballAfterTime(0.5f); //Lag compensation for fireball
                return;
            }
        }
        // Get the client ID of the object that was hit
        ulong targetClientId = targetNetworkObject.OwnerClientId;

        // Check if fireball hit its own shooter
        if (shooterID.Value == targetClientId) 
        {
            return;
        }

        if (team.Value == 0)
        {   
            //Try to get value from player
            team.Value = (int)player.getPlayersTeam() + 1;

        }
        // Team check: only damage if the player is from a different team
        if (target.getPlayersTeam() != getFireballTeam())
        {
            target.TakeDamage(damage);
            Debug.Log("Damage Dealt: " + damage);

            // Tell the shooter they successfully hit
            
            if (player != null) player.OnSuccessfulHit();

            // Destroy the fireball ONLY on the owner’s client
            deleteObjectServerRpc();
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerServerRpc(ulong shooterClientId)
    {
        if (!IsServer) return;

        shooterID.Value = shooterClientId;

        player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[shooterClientId].GetComponent<CharacterClass>();

        if (player == null)
        {
            Debug.LogError("Failed to assign player to Fireball!");
        }
    }

    public void setPlayer(CharacterClass player, ulong shooterClientId)
    {
        this.player = player;
        shooterID.Value = shooterClientId;
    }

    [ServerRpc(RequireOwnership = false)]
    private void deleteObjectServerRpc()
    {
        NetworkObject.Despawn();

        if (gameObject != null) Destroy(gameObject);
    }

    IEnumerator deleteFireballAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        deleteObjectServerRpc();
    }

    private CharacterClass.PlayerTeam getFireballTeam(){
        if (team.Value == 1){
            return CharacterClass.PlayerTeam.Fire;
        } else if (team.Value == 2){
            return CharacterClass.PlayerTeam.Water;
        }
        else {
            return CharacterClass.PlayerTeam.Fire;
        }
    }

    private void AssignPlayer()
    {
        if (player == null && shooterID.Value < 0)
        {
            player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[shooterID.Value].GetComponent<CharacterClass>();
            if (player == null)
            {
                Debug.LogError("Failed to assign player to Fireball!");
            }
        }
    }
}
