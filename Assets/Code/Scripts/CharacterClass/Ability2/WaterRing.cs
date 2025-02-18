using UnityEngine;

public class WaterRing : MonoBehaviour
{
    public float expansionTime = 5f;
    public float maxScale = 5f;
    public float rotationspeed = 180f; //Degrees per second
    public float slowerrotation = 30f;
    public float holdDuration = 1f; //Time before disappearing

    private float currentTime = 0f;
    private bool hasExpanded = false;
    private Vector3[] initialOffsets;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialOffsets = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            initialOffsets[i] = child.localPosition.normalized;
        }
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

            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.localPosition = initialOffsets[i] * (maxScale * 0.3f);
            }
            float currentRotationSpeed = Mathf.Lerp(rotationspeed, slowerrotation, progress);
            transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime);

            if (progress >= 1f)
            {
                hasExpanded = true;
                Invoke("DestroyRing", holdDuration);
            }
        }
        else
        {
            transform.Rotate(Vector3.up, slowerrotation * Time.deltaTime);
        }


    }

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
    }

    void DestroyRing()
    {
        Destroy(gameObject);
    }
}
