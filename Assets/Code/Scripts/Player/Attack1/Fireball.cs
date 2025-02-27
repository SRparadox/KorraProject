using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (collision.gameObject.GetComponent<CharacterClass>() != null)
        {
            collision.gameObject.GetComponent<CharacterClass>().TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (tag == "Fire" || tag == "Water")
        {
            return;
        }

        if (tag == "Enemy")
        {
            Debug.Log("Enemy hit!");
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}
