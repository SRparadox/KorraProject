using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

public class Sprint_Camera_effect : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera;
    public PlayerInput input;
    public float normalFollowDistance = 5f;
    public float sprintFollowDistance = 8f;
    public float transitionSpeed = 2f;

    private InputAction sprintAction;
    private Cinemachine3rdPersonFollow thirdPersonFollow;
    public ParticleSystem windeffect;

    void Awake()
    {
        windeffect.Stop();
        sprintAction = input.actions["Sprint"];
        if (sprintAction == null)
        {
            Debug.LogError("Sprint action not found! Check your Input Actions asset and ensure the name is 'Sprint'.");
        }

        thirdPersonFollow = cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (thirdPersonFollow == null)
        {
            Debug.LogError("Cinemachine3rdPersonFollow component not found on the CinemachineVirtualCamera.");
        }
    }

    void Update()
    {
        if (thirdPersonFollow == null || sprintAction == null) return;

        // Smoothly adjust the camera's follow distance based on whether the sprint action is pressed
        float targetFollowDistance = sprintAction.IsPressed() ? sprintFollowDistance : normalFollowDistance;
        thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, targetFollowDistance, Time.deltaTime * transitionSpeed);

        if (sprintAction.IsPressed())
        {
            if (!windeffect.isPlaying)
            {
                windeffect.Play(); // Start the wind effect if not already playing
            }
        }
        else
        {
            if (windeffect.isPlaying)
            {
                windeffect.Stop(); // Stop the wind effect if it is playing
            }
        }
    }
}
