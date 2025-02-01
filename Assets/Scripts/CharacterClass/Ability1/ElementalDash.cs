using System.Collections;
using UnityEngine;

public class ElementalDash: MonoBehaviour
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    [SerializeField] ParticleSystem vfxTrail;

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
        vfxTrail.Play();

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            characterController.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }

        vfxTrail.Stop();
    }
}
