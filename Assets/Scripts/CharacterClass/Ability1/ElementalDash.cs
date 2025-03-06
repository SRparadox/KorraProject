using System.Collections;
using UnityEngine;

public class ElementalDash: MonoBehaviour
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    [SerializeField] private ParticleSystem vfxTrailPrefab; // Reference to the particle system prefab

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Trigger()
    {
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        // Instantiate the VFX trail prefab and set the parent to the character
        ParticleSystem vfxTrail = Instantiate(vfxTrailPrefab, transform.position, Quaternion.identity);
        vfxTrail.transform.SetParent(transform); // Set parent to the character

        vfxTrail.Play();

        float startTime = Time.time;

        // Perform the dash while the duration is not finished
        while (Time.time < startTime + dashDuration)
        {
            characterController.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(vfxTrail.gameObject, vfxTrail.main.duration);
        vfxTrail.Stop();
    }
}
