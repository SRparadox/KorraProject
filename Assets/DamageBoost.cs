using System.Collections;
using UnityEngine;

public class DamageBoost : MonoBehaviour
{
    public float DamageMultiplier = 1.5f;
    public float Duration = 5.0f;
    private bool isActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateBoost(){
        if (!isActive)
        {
            Debug.Log("Activated Damage Boost");
            isActive = true;
            StartCoroutine(DeactivateBoostAfterDelay());
            
        }
    }

    private IEnumerator DeactivateBoostAfterDelay()
    {
        yield return new WaitForSecondsRealtime(Duration);
        isActive = false;
        Debug.Log("Deactivated Damage Boost");
    }

    public float getDamageBoost(){
        if (isActive) {
            return DamageMultiplier;
        }
        else {
            return 1.0f;
        }
    }

}
