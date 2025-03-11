using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{

    TextMeshPro text;
    float damageValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.text = "0";
    }

    public void setDamageText(float damage)
    {
        damageValue = damage;
    }

    // Update is called once per frame
    void Update()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.01f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
        if (text.color.a <= 0)
        {
            Destroy(gameObject);
        }
        if (text.text != damageValue.ToString())
        {
            text.text = damageValue.ToString();
        }
    }
}
