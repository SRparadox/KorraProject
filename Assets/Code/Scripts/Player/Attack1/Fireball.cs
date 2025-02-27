using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 20;

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

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
