using UnityEngine;

public class Ultimate: MonoBehaviour
{
    public GameObject fireRingPrefab;
    public GameObject waterRingPrefab;
    public float expansionTime = 5f;
    public float maxScale = 8f;
    public float Speed = 20f;
    public float lifeTime = 5f;
    public float damage = 75;

    private string playertag;
    private float currentTime = 0f;
    private bool hasExpanded = false;
    private Rigidbody rb;
    private Vector3 launchDirection;
    private UltimateAttack attackScript;

    public void Initialize(string tag)
    {
        playertag = tag;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        transform.localScale = Vector3.zero;
    }
    public void StartExpansion(Vector3 direction, UltimateAttack attack)
    {
        launchDirection = direction;
        attackScript = attack;
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

            ScaleAllChildren(transform, scaleFactor);

            if (progress >= 1f)
            {
                hasExpanded = true;
                LaunchForward();
            }
        }
    }

    void LaunchForward()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = launchDirection * Speed;
        }
        Debug.Log("Ultimate launched! Resetting camera...");
        attackScript.ResetCamera();
        Destroy(gameObject, lifeTime);
    }

    void ScaleAllChildren(Transform parent, float scaleFactor)
    {
        foreach (Transform child in parent)
        {
            child.localScale = Vector3.one * scaleFactor;
            ScaleAllChildren(child, scaleFactor);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterClass>() != null)
        {
            collision.gameObject.GetComponent<CharacterClass>().TakeDamage(damage);
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Vector3 impactPosition = collision.contacts[0].point;
            GameObject ringToSpawn = null;

            if (playertag == "Fire" && fireRingPrefab != null)
            {
                ringToSpawn = fireRingPrefab;
            } else if (playertag == "Water" && waterRingPrefab != null)
            {
                ringToSpawn = waterRingPrefab;
            }

            if (ringToSpawn != null)
            {
                Instantiate(ringToSpawn, impactPosition, Quaternion.identity);
            } else
            {
                Debug.LogError("No valid ring prefab assigned for " + playertag);
            }

            Destroy(gameObject);
        }
    }
}
