using UnityEngine;

public class WaterRing : MonoBehaviour
{
    public float expansionTime = 5f;
    public float maxScale = 5f;
    public float rotationspeed = 180f; //Degrees per second
    public float slowerrotation = 30f;
    public float holdDuration = 1f; //Time before disappearing

    public float damage = 25;

    private float currentTime = 0f;
    private bool hasExpanded = false;
    private Vector3[] initialOffsets;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasExpanded)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentTime / expansionTime);
            float scaleFactor = Mathf.Lerp(0.1f, maxScale, progress);
            transform.localScale = Vector3.one * scaleFactor; 

            float currentRotationSpeed = Mathf.Lerp(rotationspeed, slowerrotation, progress);
            transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime, Space.World);

            if (progress >= 1f)
            {
                hasExpanded = true;
                Invoke("DestroyRing", holdDuration);
            }
        }
        else
        {
            transform.Rotate(Vector3.up, slowerrotation * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterClass>() != null)
        {
            collision.gameObject.GetComponent<CharacterClass>().TakeDamage(damage);
            return;
        }

    }

    void DestroyRing()
    {
        Destroy(gameObject);
    }
}
