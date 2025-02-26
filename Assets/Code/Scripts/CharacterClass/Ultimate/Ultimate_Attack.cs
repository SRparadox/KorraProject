using Cinemachine;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Ultimate_Attack : MonoBehaviour
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
        followCamera = (CinemachineVirtualCamera)FindFirstObjectByType(typeof(CinemachineVirtualCamera));
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
      
        if(selectedPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 6, 0);
            GameObject ultimate = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            if(followCamera != null)
            {
                followCamera.enabled = false;
            }

            originalCameraPosition = cameraTransform.position;
            isZoomingOut = true;
            isReturning = false;

            Ultimate ultimateScript = ultimate.GetComponent<Ultimate>();
            if(ultimateScript != null)
            {
                ultimateScript.Initialize(gameObject.tag);
                ultimateScript.StartExpansion(transform.forward, this);
            }
            Debug.Log(selectedPrefab.name + "Spawned");
        }
        else
        {
            Debug.LogError("Ultimate prefab not assigned in the Inspector");
        }
    }

    private void Update()
    {
        Debug.Log("Camera Movement Update: isZoomingOut=" + isZoomingOut + " | isReturning=" + isReturning);
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        if (isZoomingOut)
        {
            Vector3 targetPosition = originalCameraPosition - cameraTransform.forward * zoomOutDistance + new Vector3(0, 3, 0);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * transitionSpeed);

            if(Vector3.Distance(cameraTransform.position, targetPosition) < 0.1f)
            {
                isZoomingOut = false;
            }
        }
        else if(isReturning)
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

        if(followCamera != null)
        {
            followCamera.enabled = true;
        }
    }

    public void SetPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

}
