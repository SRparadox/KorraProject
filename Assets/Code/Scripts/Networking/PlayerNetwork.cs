using Cinemachine;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork: NetworkBehaviour
{
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private CharacterClass characterClass;

    private CinemachineVirtualCamera[] virtualCameras;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        characterClass = GetComponent<CharacterClass>();

        // Find all CinemachineVirtualCamera components in children
        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>(true);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Debug.Log("Not owner");

            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = false;
            if (thirdPersonController != null)
                thirdPersonController.enabled = false;
            if (characterClass != null)
                characterClass.enabled = false;

            // Disable all cameras for non-local players
            foreach (var cam in virtualCameras)
            {
                cam.gameObject.SetActive(false);
            }
        } else
        {
            Debug.Log("Owner");

            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = true;
            if (thirdPersonController != null)
                thirdPersonController.enabled = true;
            if (characterClass != null)
                characterClass.enabled = true;

            // Enable all cameras for local player
            foreach (var cam in virtualCameras)
            {
                cam.gameObject.SetActive(true);
            }
        }
    }
}
