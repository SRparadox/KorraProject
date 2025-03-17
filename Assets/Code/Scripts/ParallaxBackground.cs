using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code from ChatGPT

public class ParrallaxBackground: MonoBehaviour
{
    public float parrallaxStrength = 0.1f;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition.x /= Screen.width;
        mousePosition.y /= Screen.height;

        Vector3 offset = new Vector3(
            (mousePosition.x - 0.5f) * parrallaxStrength,
            (mousePosition.y - 0.5f) * parrallaxStrength,
            0);

        transform.position = initialPosition + offset;
    }
}