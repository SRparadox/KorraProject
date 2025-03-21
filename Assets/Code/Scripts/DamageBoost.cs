using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DamageBoost: NetworkBehaviour
{
    public ParticleSystem particleSystem;
    //sync damage multiplier with network
    public NetworkVariable<float> damageMultiplier = new NetworkVariable<float>(
        1.0f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );
    public float increaseMultiplierBy = 0.5f;
    public float Duration = 5.0f;
    private bool isActive = false;

    [ServerRpc(RequireOwnership = false)]
    void startParticleSystemServerRpc()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    public void ActivateBoost()
    {
        if (!IsOwner) return; // Only the owner can activate the boost
        if (!isActive)
        {
            Debug.Log("Activated Damage Boost");
            isActive = true;
            damageMultiplier.Value += increaseMultiplierBy;
            startParticleSystemServerRpc();
            StartCoroutine(DeactivateBoostAfterDelay());

        }
    }

    private IEnumerator DeactivateBoostAfterDelay()
    {
        yield return new WaitForSecondsRealtime(Duration);
        isActive = false;
        damageMultiplier.Value -= increaseMultiplierBy;
        Debug.Log("Deactivated Damage Boost");
    }

    public float getDamageBoost()
    {
        if (isActive)
        {
            Debug.Log("Damage Multiplier: " + damageMultiplier.Value);
            return damageMultiplier.Value;
        } else
        {
            return 1.0f;
        }
    }
}
