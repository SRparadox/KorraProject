using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameObject flameEffect_Prefab;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Fireball collided with: " + collision.gameObject.name);

        if (flameEffect_Prefab != null)
        {
            GameObject flame = Instantiate(flameEffect_Prefab, collision.contacts[0].point, Quaternion.identity);

            Destroy(flame, 7f);
        }

        

        Destroy(gameObject);
    }
}
