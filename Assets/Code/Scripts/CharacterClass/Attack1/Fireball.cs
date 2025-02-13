using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameObject flameEffect_Prefab;
    public int damage = 20;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
        }

        

        //Destroy(gameObject);
    }
}
