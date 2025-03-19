using Cinemachine;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork: NetworkBehaviour
{
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private PlayerInput playerInput;
    private CinemachineVirtualCamera[] virtualCameras;
    private Transform cameraRoot;

    private void Awake()
    {
        TryGetComponent(out thirdPersonController);
        TryGetComponent(out starterAssetsInputs);
        TryGetComponent(out playerInput);

        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>(true);
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

        // Enable player input and movement for the local player
        starterAssetsInputs.enabled = true;
        thirdPersonController.enabled = true;
        playerInput.enabled = true;

        // Activate cameras
        foreach (var cam in virtualCameras)
        {
            cam.gameObject.SetActive(true);
        }

        if (cameraRoot != null)
            cameraRoot.gameObject.SetActive(true);
    }

    private void EnableRemotePlayer()
    {
        Debug.Log("SPAWNED REMOTE PLAYER");

        // Disable player input and cameras for remote players
        playerInput.enabled = false;
        foreach (var cam in virtualCameras)
        {
            cam.gameObject.SetActive(false);
        }

        if (cameraRoot != null)
            cameraRoot.gameObject.SetActive(false);

        // Allow movement simulation on the server for remote clients
        bool shouldEnableMovement = IsServer;
        thirdPersonController.enabled = shouldEnableMovement;
        starterAssetsInputs.enabled = shouldEnableMovement;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        // Send player input to the server
        SubmitInputServerRpc(starterAssetsInputs.move, starterAssetsInputs.jump, starterAssetsInputs.sprint);
    }

    [ServerRpc]
    private void SubmitInputServerRpc(Vector2 moveInput, bool jump, bool sprint)
    {
        // Ensure the server does not overwrite owner-controlled movement
        if (!IsOwner)
            return;

        // Sync input values for movement simulation on the server
        starterAssetsInputs.move = moveInput;
        starterAssetsInputs.jump = jump;
        starterAssetsInputs.sprint = sprint;
    }
}
