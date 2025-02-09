using UnityEngine;

public class WaterRing : MonoBehaviour
{
    public float expansionTime = 5f;
    public float maxScale = 5f;
    private float currentTime = 0f;
    private Vector3 intialScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        intialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.deltaTime;

        float scaleFactor = Mathf.Lerp(0.1f, maxScale, currentTime / expansionTime);
        transform.localScale = new Vector3(scaleFactor, scaleFactor,scaleFactor);

        if(currentTime >= expansionTime)
        {
            Destroy(gameObject);
        }
    }
}
