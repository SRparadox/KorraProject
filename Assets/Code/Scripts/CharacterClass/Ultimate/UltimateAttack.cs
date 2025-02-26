using Cinemachine;
<<<<<<< HEAD:Assets/Code/Scripts/CharacterClass/Ultimate/UltimateAttack.cs
=======
using TMPro;
using UnityEditor.Experimental.GraphView;
>>>>>>> origin/Bryon_Branch:Assets/Code/Scripts/CharacterClass/Ultimate/Ultimate_Attack.cs
using UnityEngine;

public class UltimateAttack: MonoBehaviour
{
    public float zoomOutDistance = 15f;
    public float transitionSpeed = 2f;

    private GameObject selectedPrefab;
    private Vector3 originalCameraPosition;
    private bool isZoomingOut = false;
    private bool isReturning = false;
    private Transform cameraTransform;
    private CinemachineVirtualCamera followCamera;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        followCamera = (CinemachineVirtualCamera) FindFirstObjectByType(typeof(CinemachineVirtualCamera));
        if (followCamera == null)
        {
            Debug.LogError("Main Camera not found! Ensure your camera is tagged as 'MainCamera'.");
        }
        originalCameraPosition = cameraTransform.position;
    }

    public void Trigger()
    {
        SpawnUltimate();
    }

    private void SpawnUltimate()
    {

        if (selectedPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 6, 0);
            GameObject ultimate = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            if (followCamera != null)
            {
                followCamera.enabled = false;
            }

            originalCameraPosition = cameraTransform.position;
            isZoomingOut = true;
            isReturning = false;

            Ultimate ultimateScript = ultimate.GetComponent<Ultimate>();
            if (ultimateScript != null)
            {
                ultimateScript.Initialize(gameObject.tag);
                ultimateScript.StartExpansion(transform.forward, this);
            }
            Debug.Log(selectedPrefab.name + "Spawned");
        } else
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
<<<<<<< HEAD:Assets/Code/Scripts/CharacterClass/Ultimate/UltimateAttack.cs
            Vector3 targetPosition = originalCameraPosition - new Vector3(0, -3, zoomOutDistance);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * transitionSpeed);
        } else if (isReturning)
=======
            Vector3 targetPosition = originalCameraPosition - cameraTransform.forward * zoomOutDistance + new Vector3(0, 3, 0);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * transitionSpeed);

            if(Vector3.Distance(cameraTransform.position, targetPosition) < 0.1f)
            {
                isZoomingOut = false;
            }
        }
        else if(isReturning)
>>>>>>> origin/Bryon_Branch:Assets/Code/Scripts/CharacterClass/Ultimate/Ultimate_Attack.cs
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, originalCameraPosition, Time.deltaTime * transitionSpeed);
            if (Vector3.Distance(cameraTransform.position, originalCameraPosition) < 0.1f)
            {
                isReturning = false;
                if(followCamera != null)
                {
                    followCamera.enabled = true;
                }
            }
        }
    }

    public void ResetCamera()
    {
        isZoomingOut = false;
        isReturning = true;

        if (followCamera != null)
        {
            followCamera.enabled = true;
        }
    }

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

}
