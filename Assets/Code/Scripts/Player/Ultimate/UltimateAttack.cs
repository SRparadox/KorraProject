using Unity.Netcode;
using Cinemachine;
using UnityEngine;

public class UltimateAttack : NetworkBehaviour
{
    public float zoomOutDistance = 15f;
    public float transitionSpeed = 2f;
    private CharacterClass characterClass;
    private GameObject selectedPrefab;
    private Vector3 originalCameraPosition;
    private bool isZoomingOut = false;
    private bool isReturning = false;
    private Transform cameraTransform;
    private CinemachineVirtualCamera followCamera;

    private void Start()
    {
        characterClass = GetComponent<CharacterClass>();
        cameraTransform = Camera.main.transform;
        followCamera = (CinemachineVirtualCamera)FindObjectOfType(typeof(CinemachineVirtualCamera));
        if (followCamera == null)
            Debug.LogError("Main Camera not found! Ensure your camera is tagged as 'MainCamera'.");
        originalCameraPosition = cameraTransform.position;
    }

    public void Trigger()
    {
        if (IsOwner)
            SpawnUltimateServerRpc();
    }

    [ServerRpc]
    private void SpawnUltimateServerRpc()
    {
        SpawnUltimate();
    }

    private void SpawnUltimate()
    {
        if (selectedPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 6, 0);
            Vector3 direction = cameraTransform.forward;
            GameObject ultimate = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            var netObj = ultimate.GetComponent<NetworkObject>();
            if (netObj != null)
                netObj.Spawn();
            if (followCamera != null)
                followCamera.enabled = false;
            originalCameraPosition = cameraTransform.position;
            isZoomingOut = true;
            isReturning = false;
            Ultimate ultimateScript = ultimate.GetComponent<Ultimate>();
            if (ultimateScript != null)
            {
                ultimateScript.Initialize(characterClass.getPlayersTeam());
                ultimateScript.StartExpansion(direction, this);
            }
            Debug.Log(selectedPrefab.name + " Spawned");
        }
        else
        {
            Debug.LogError("Ultimate prefab not assigned in the Inspector");
        }
    }

    private void Update()
    {
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        if (isZoomingOut)
        {
            Vector3 targetPosition = originalCameraPosition - cameraTransform.forward * zoomOutDistance + new Vector3(0, 3, 0);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * transitionSpeed);
            if (Vector3.Distance(cameraTransform.position, targetPosition) < 0.1f)
                isZoomingOut = false;
        }
        else if (isReturning)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, originalCameraPosition, Time.deltaTime * transitionSpeed);
            if (Vector3.Distance(cameraTransform.position, originalCameraPosition) < 0.1f)
            {
                isReturning = false;
                if (followCamera != null)
                    followCamera.enabled = true;
            }
        }
    }

    public void ResetCamera()
    {
        isZoomingOut = false;
        isReturning = true;
        if (followCamera != null)
            followCamera.enabled = true;
    }

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }
}
