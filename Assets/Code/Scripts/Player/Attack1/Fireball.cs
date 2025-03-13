using Unity.Netcode;
using UnityEngine;

public class Fireball : NetworkBehaviour
{
    public int damage = 10;
    private CharacterClass player;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        CharacterClass target = collision.gameObject.GetComponent<CharacterClass>();
        if (target != null && target.getPlayersTeam() != player.getPlayersTeam())
        {
            target.TakeDamage(damage * player.getDamageMultiplier());
            Debug.Log("Damage Dealt: " + damage * player.getDamageMultiplier());
            Destroy(gameObject);
            if (player != null)
                player.OnSuccessfulHit();
            return;
        }
        Destroy(gameObject);
    }

    public void SetPlayer(CharacterClass character)
    {
        player = character;
    }
}
