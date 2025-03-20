using UnityEngine;
using Unity.Netcode;
using UnityEditor.Rendering;

public class Fireball : NetworkBehaviour
{
    public int damage = 10;
    private CharacterClass player;

    // Store the shooter's NetworkObject ID
    private NetworkVariable<ulong> shooterID = new NetworkVariable<ulong>();

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return; // Only server handles damage

        if (player == null)
        {
            AssignPlayer();
            if (player == null)
            {
                Debug.LogError("Fireball's shooter is missing!");
                Destroy(gameObject);
                return;
            }
        }

        CharacterClass.PlayerTeam team = player.getPlayersTeam();
        CharacterClass target = collision.gameObject.GetComponent<CharacterClass>();

        if (target != null && target.getPlayersTeam() != team)
        {
            target.TakeDamage(damage * player.getDamageMultiplier());
            Debug.Log("Damage Dealt: " + damage * player.getDamageMultiplier());
            Destroy(gameObject);
            player.OnSuccessfulHit();
            return;
        }

        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerServerRpc(ulong shooterClientId)
    {
        if (!IsServer) return;

        shooterID.Value = shooterClientId;

        player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[shooterClientId].GetComponent<CharacterClass>();
        Debug.Log(player);

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
