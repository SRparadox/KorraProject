using System.Collections;
using UnityEngine;

public class DamageBoost: MonoBehaviour
{
    public ParticleSystem particleSystem;
    float DamageMultiplier = 1.0f;
    public float increaseMultiplierBy = 0.5f;
    public float Duration = 5.0f;
    private bool isActive = false;

    void startParticleSystem()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    public void ActivateBoost()
    {
        if (!isActive)
        {
            Debug.Log("Activated Damage Boost");
            isActive = true;
            DamageMultiplier += increaseMultiplierBy;
            startParticleSystem();
            StartCoroutine(DeactivateBoostAfterDelay());

        }
    }

    private IEnumerator DeactivateBoostAfterDelay()
    {
        yield return new WaitForSecondsRealtime(Duration);
        isActive = false;
        DamageMultiplier -= increaseMultiplierBy;
        Debug.Log("Deactivated Damage Boost");
    }

    public float getDamageBoost()
    {
        if (isActive)
        {
            Debug.Log("Damage Multiplier: " + DamageMultiplier);
            return DamageMultiplier;
        } else
        {
            return 1.0f;
        }
    }
}
