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

    // Network variables for synchronization
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(
        writePerm: NetworkVariableWritePermission.Owner);

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

    private void Update()
    {
        if (IsOwner)
        {
            // Send input to the server
            SubmitInputServerRpc(
                starterAssetsInputs.move,
                starterAssetsInputs.look,
                starterAssetsInputs.jump,
                starterAssetsInputs.sprint,
                starterAssetsInputs.aim,
                starterAssetsInputs.attack,
                starterAssetsInputs.selectedAbility
            );

            // Update networked position and rotation
            networkPosition.Value = transform.position;
            networkRotation.Value = transform.rotation;
        } else
        {
            // Apply received position and rotation from the network
            transform.position = networkPosition.Value;
            transform.rotation = networkRotation.Value;
        }
    }

    [ServerRpc]
    private void SubmitInputServerRpc(Vector2 moveInput, Vector2 lookInput, bool jump, bool sprint, bool aim, bool attack, int selectedAbility)
    {
        if (!IsOwner)
            return;

        // Apply input on the player
        starterAssetsInputs.move = moveInput;
        starterAssetsInputs.look = lookInput;
        starterAssetsInputs.jump = jump;
        starterAssetsInputs.sprint = sprint;
        starterAssetsInputs.aim = aim;
        starterAssetsInputs.attack = attack;
        starterAssetsInputs.selectedAbility = selectedAbility;
    }
}
