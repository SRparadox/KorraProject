using UnityEngine;
using Unity.Netcode; // Import Netcode for GameObjects
using StarterAssets;  
// This script wraps your existing player components.
public class PlayerNetwork : NetworkBehaviour
{
    // References to your existing components on the player prefab
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private CharacterClass characterClass;

    private void Awake()
    {
        // Get references to the existing scripts on the same GameObject.
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        characterClass = GetComponent<CharacterClass>();
    }

    public override void OnNetworkSpawn()
    {
        // This method is called when the object is spawned on the network.
        // Only the local player (owner) should process input and control movement.
        if (!IsOwner)
        {
            // Disable input & movement components for non-owner instances.
            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = false;
            if (thirdPersonController != null)
                thirdPersonController.enabled = false;
            if (characterClass != null)
                characterClass.enabled = false;
        }
        else
        {
            // For the local owner, ensure these components are enabled.
            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = true;
            if (thirdPersonController != null)
                thirdPersonController.enabled = true;
            if (characterClass != null)
                characterClass.enabled = true;
        }
    }
}
