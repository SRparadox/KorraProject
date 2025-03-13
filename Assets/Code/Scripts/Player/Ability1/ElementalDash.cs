using System.Collections;
using UnityEngine;

public class ElementalDash : MonoBehaviour
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
        if (selectedPrefab == null)
        {
            Debug.LogError("ElementalDash: selectedPrefab is null on " + gameObject.name);
            yield break;
        }

        ParticleSystem vfxTrail = Instantiate(selectedPrefab, transform.position + Vector3.down * 0.3f, Quaternion.identity);
        vfxTrail.transform.SetParent(transform);
        vfxTrail.Play();

        float startTime = Time.time;
        Vector3 dashDirection = (transform.forward + Vector3.up * upwardVelocity).normalized;

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
        if (prefab == null)
        {
            Debug.LogError("ElementalDash: Attempted to set a null dash prefab on " + gameObject.name);
            return;
        }
        Debug.Log("Setting Dash Prefab");
        selectedPrefab = prefab;
    }
}
