using Mono.Cecil.Cil;
using UnityEngine;
using System.Collections;

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

    private void OnTriggerEnter(Collider other)
    {
        CharacterClass character = other.GetComponent<CharacterClass>();
        if (character == null) return;

        string myTeam = transform.root.tag;
        string otherTeam = other.tag;

        if (!other.CompareTag(myTeam))
        {
            Debug.Log("Applying Damage and Knockback to: " + other.name);
            character.TakeDamage(damage);

            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            knockbackDirection.y = 0.1f;
            float knockbackDistance = 7f; 
            float knockbackDuration = 0.2f; 

            StartCoroutine(Knockback(other.transform, knockbackDirection, knockbackDistance, knockbackDuration));
            Debug.Log("Knockback Applied");
        }
    }

    private IEnumerator Knockback(Transform target, Vector3 direction, float distance, float duration)
    {
        Vector3 startPosition = target.position;
        Vector3 endPosition = startPosition + direction * distance;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.position = Vector3.MoveTowards(target.position, endPosition, (distance / duration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.position = endPosition;
    }

    void DestroyRing()
    {
        Destroy(gameObject);
    }
}
