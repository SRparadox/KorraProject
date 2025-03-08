using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 10;
    private CharacterClass player;
    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (collision.gameObject.GetComponent<CharacterClass>() != null && collision.gameObject.tag != gameObject.tag)
        {
            collision.gameObject.GetComponent<CharacterClass>().TakeDamage(damage * player.getDamageMultiplier());
            Debug.Log("Damage Dealt: " + damage * player.getDamageMultiplier());
            Destroy(gameObject);
            if (player != null)
            {
                player.OnSuccessfulHit();
            }
            return;
        }

        Destroy(gameObject);
    }

    public void SetPlayer(CharacterClass character)
    {
        player = character;
    }
}
