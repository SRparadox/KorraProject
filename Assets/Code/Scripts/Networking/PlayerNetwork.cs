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

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        playerInput = GetComponent<PlayerInput>();

        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>(true);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Debug.Log($"[PlayerNetwork] {gameObject.name} is NOT the local player. Disabling components...");

            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = false;
            if (thirdPersonController != null)
                thirdPersonController.enabled = false;
            if (playerInput != null)
                playerInput.enabled = false;

            foreach (var cam in virtualCameras)
            {
                cam.gameObject.SetActive(false);
            }

            Transform cameraRoot = transform.Find("PlayerCameraRoot");
            if (cameraRoot != null)
            {
                cameraRoot.gameObject.SetActive(false);
            }
        } else
        {
            Debug.Log($"[PlayerNetwork] {gameObject.name} is the local player. Enabling components...");

            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = true;
            if (thirdPersonController != null)
                thirdPersonController.enabled = true;
            if (playerInput != null)
                playerInput.enabled = true;

            foreach (var cam in virtualCameras)
            {
                cam.gameObject.SetActive(true);
            }
        }
    }
}
