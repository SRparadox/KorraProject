using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ElementalDash: NetworkBehaviour
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float upwardVelocity = 0.5f;

    private ParticleSystem selectedPrefab;

    private CharacterController characterController;
    private Animator animator;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void Trigger()
    {
        if (!IsOwner) return; // Only the owner can trigger the dash
        // Perform the dash locally
        StartCoroutine(Dash());
        StartDashServerRpc();
    }

    [ServerRpc]
    private void StartDashServerRpc(){
        // Tell all clients to play the dash VFX
        PlayDashVFXClientRpc();
    }

    private IEnumerator Dash()
    {
        float startTime = Time.time;

        Vector3 dashDirection = transform.forward + Vector3.up * upwardVelocity;
        dashDirection.Normalize();

        // Set the animator state to "dashing"
        animator.SetBool("IsDashing", true);

        while (Time.time < startTime + dashDuration)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        // Stop the dash animation
        animator.SetBool("IsDashing", false);
    }

    [ClientRpc]
    private void PlayDashVFXClientRpc()
    {
        if (selectedPrefab == null) return;

        // Instantiate the VFX on each client
        ParticleSystem vfxTrail = Instantiate(selectedPrefab, transform.position + Vector3.down * 0.3f, Quaternion.identity);
        vfxTrail.transform.SetParent(transform);
        vfxTrail.Play();

        // Destroy the VFX after its duration
        Destroy(vfxTrail.gameObject, vfxTrail.main.duration);
    }

    public void SetPrefab(ParticleSystem prefab)
    {
        selectedPrefab = prefab;
    }
}
