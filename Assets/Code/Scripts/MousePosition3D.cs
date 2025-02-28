using UnityEngine;

// code from https://youtu.be/0jTPKz3ga4w?feature=shared

public class MousePosition3D: MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask ignoreLayer;

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, ~ignoreLayer))
        {
            transform.position = raycastHit.point;
        }
    }
}
