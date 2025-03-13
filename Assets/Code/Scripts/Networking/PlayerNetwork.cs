using UnityEngine;
using Unity.Netcode;
using StarterAssets;

public class PlayerNetwork : NetworkBehaviour
{
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private CharacterClass characterClass;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        characterClass = GetComponent<CharacterClass>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = false;
            if (thirdPersonController != null)
                thirdPersonController.enabled = false;
            if (characterClass != null)
                characterClass.enabled = false;
        }
        else
        {
            if (starterAssetsInputs != null)
                starterAssetsInputs.enabled = true;
            if (thirdPersonController != null)
                thirdPersonController.enabled = true;
            if (characterClass != null)
                characterClass.enabled = true;
        }
    }
}
