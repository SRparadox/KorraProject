using System.Collections;
using UnityEngine;

public class ElementalDash: MonoBehaviour
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
        Debug.Log("Begin Dashing");
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        ParticleSystem vfxTrail = Instantiate(selectedPrefab, transform.position + Vector3.down * 0.3f, Quaternion.identity);
        vfxTrail.transform.SetParent(transform);

        vfxTrail.Play();

        float startTime = Time.time;

        Vector3 dashDirection = transform.forward + Vector3.up * upwardVelocity;
        dashDirection.Normalize();

        while (Time.time < startTime + dashDuration)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
        
        animator.SetBool("IsDashing", false);
        Destroy(vfxTrail.gameObject, vfxTrail.main.duration);
        vfxTrail.Stop();
    }

    public void SetPrefab(ParticleSystem prefab)
    {
        Debug.Log("Setting Dash Prefab");
        selectedPrefab = prefab;
    }
}
