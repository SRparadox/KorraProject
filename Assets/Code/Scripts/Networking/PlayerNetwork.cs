using Cinemachine;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork: NetworkBehaviour
{
    private CharacterController characterController;
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private PlayerInput playerInput;
    private CinemachineVirtualCamera[] virtualCameras;
    private Transform cameraRoot;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        playerInput = GetComponent<PlayerInput>();

        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        cameraRoot = transform.Find("PlayerCameraRoot");
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            EnableLocalPlayer();
        } else
        {
            EnableRemotePlayer();
        }
    }

    private void EnableLocalPlayer()
    {
        Debug.Log("SPAWNED LOCAL PLAYER");
    }

    private void EnableRemotePlayer()
    {
        Debug.Log("SPAWNED REMOTE PLAYER");

        // Disable components
        if (characterController != null)
            characterController.enabled = false;
        if (thirdPersonController != null)
            thirdPersonController.enabled = false;
        if (starterAssetsInputs != null)
            starterAssetsInputs.enabled = false;
        if (playerInput != null)
            playerInput.enabled = false;

        // Disable all virtual cameras
        if (virtualCameras != null)
        {
            foreach (var cam in virtualCameras)
            {
                if (cam != null)
                    cam.gameObject.SetActive(false);
            }
        }

        // Disable camera root
        if (cameraRoot != null)
            cameraRoot.gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if (!IsOwner)
    //        return;

    //    // Send player input to the server
    //    SubmitInputServerRpc(starterAssetsInputs.move, starterAssetsInputs.jump, starterAssetsInputs.sprint);
    //}

    //[ServerRpc]
    //private void SubmitInputServerRpc(Vector2 moveInput, bool jump, bool sprint)
    //{
    //    // Ensure the server does not overwrite owner-controlled movement
    //    if (!IsOwner)
    //        return;

    //    // Sync input values for movement simulation on the server
    //    starterAssetsInputs.move = moveInput;
    //    starterAssetsInputs.jump = jump;
    //    starterAssetsInputs.sprint = sprint;
    //}
}
