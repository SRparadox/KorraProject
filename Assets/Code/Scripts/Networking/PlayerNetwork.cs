using Cinemachine; // Ensure you have Cinemachine installed!
using StarterAssets;
using Unity.Netcode; // Import Netcode for GameObjects
using UnityEngine;

public class PlayerNetwork: NetworkBehaviour
{
    // References to your existing components on the player prefab.
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private CharacterClass characterClass;

    // Reference to the Cinemachine Virtual Camera attached to the player prefab.
    public CinemachineVirtualCamera playerCamera;

    private void Awake()
    {
        // Get references to the existing scripts on the same GameObject.
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        characterClass = GetComponent<CharacterClass>();

        // If the camera isn't manually assigned in the Inspector,
        // try to get it from the children.
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public override void OnNetworkSpawn()
    {
        // Only the local player's object should process input, movement, etc.
        if (!IsOwner)
        {
            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = false;
            if (thirdPersonController != null)
                thirdPersonController.enabled = false;
            if (characterClass != null)
                characterClass.enabled = false;

            // Disable the camera for remote players.
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
        } else
        {
            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = true;
            if (thirdPersonController != null)
                thirdPersonController.enabled = true;
            if (characterClass != null)
                characterClass.enabled = true;

            // Enable the local player's camera and set it to follow this player.
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
            } else
            {
                Debug.LogWarning("Local player has no camera attached!");
            }
        }
    }
}