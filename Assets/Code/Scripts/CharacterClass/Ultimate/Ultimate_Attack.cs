using Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Ultimate_Attack : MonoBehaviour
{
    public GameObject waterUltPrefab;
    public GameObject fireUltPrefab;
    public float zoomOutDistance = 15f;
    public float transitionSpeed = 2f;

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
        originalCameraPosition = cameraTransform.localPosition;
    }

    public void Trigger()
    {
        SpawnUltimate();
    }

    private void SpawnUltimate()
    {
        GameObject selectedPrefab = null;

        if (gameObject.CompareTag("Fire"))
        {
            selectedPrefab = fireUltPrefab;
        }
        else if (gameObject.CompareTag("Water"))
        {
            selectedPrefab = waterUltPrefab;
        }

        if(selectedPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 6, 0);
            GameObject ultimate = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            if(followCamera != null)
            {
                followCamera.enabled = false;
            }
            isZoomingOut = true;
            isReturning = false;

            Ultimate ultimateScript = ultimate.GetComponent<Ultimate>();
            if(ultimateScript != null)
            {
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
            Vector3 targetPosition = originalCameraPosition - new Vector3 (0,-3,zoomOutDistance);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * transitionSpeed);
        }
        else if(isReturning)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, originalCameraPosition, Time.deltaTime * transitionSpeed);
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
   
}
