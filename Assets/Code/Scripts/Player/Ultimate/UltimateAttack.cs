using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class UltimateAttack: MonoBehaviour
{
    public float zoomOutDistance = 15f;
    public float transitionSpeed = 2f;

    private GameObject selectedPrefab;
    [SerializeField] private float ballRadius = 2f;
    [SerializeField] private float chargeUpTime = 2f;

    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] float throwSpeed = 5f;
    [SerializeField] private float acceleration = 2f;

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
            Vector3 spawnPosition = transform.position + new Vector3(0, 4, 0);
            GameObject ball = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            ball.transform.SetParent(transform);

            if (followCamera != null)
            {
                followCamera.enabled = false;
            }

            originalCameraPosition = cameraTransform.position;
            isZoomingOut = true;
            isReturning = false;

            StartCoroutine(ChargeUp(ball));
        } else
        {
            Debug.LogError("Ultimate prefab not assigned in the Inspector");
        }
    }

    private IEnumerator ChargeUp(GameObject ball)
    {
        Vector3 initialScale = ball.transform.localScale;
        Vector3 targetScale = new Vector3(ballRadius, ballRadius, ballRadius);

        float elapsedTime = 0f;

        ball.transform.localScale = Vector3.zero;

        while (elapsedTime < chargeUpTime)
        {
            ball.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / chargeUpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ball.transform.localScale = targetScale;
        ball.transform.SetParent(null);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        int ignoreLayer = LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, ~ignoreLayer))
        {
            StartCoroutine(Throw(ball, raycastHit.point));
        }
    }

    private IEnumerator Throw(GameObject ball, Vector3 target)
    {
        Vector3 direction = (target - ball.transform.position).normalized;
        float currentSpeed = throwSpeed;

        while (ball != null)
        {
            ball.transform.position += direction * currentSpeed * Time.deltaTime;
            currentSpeed += acceleration * Time.deltaTime;

            if (Vector3.Distance(ball.transform.position, target) < 0.4f)
            {
                break;
            }

            yield return null;
        }

        Destroy(ball, 0.5f);
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
            {
                isZoomingOut = false;
            }
        } else if (isReturning)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, originalCameraPosition, Time.deltaTime * transitionSpeed);
            if (Vector3.Distance(cameraTransform.position, originalCameraPosition) < 0.1f)
            {
                isReturning = false;
                if (followCamera != null)
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
