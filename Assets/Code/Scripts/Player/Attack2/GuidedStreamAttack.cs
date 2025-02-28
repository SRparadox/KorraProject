using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GuidedStreamAttack: MonoBehaviour
{
    [SerializeField] GuidedStream selectedPrefab;
    [SerializeField] Camera camera;
    [SerializeField] float damage = 25;

    public void Trigger()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        int ignoreLayer = LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, ~ignoreLayer))
        {
            SpawnGuidedStream(raycastHit.point);
        }
    }

    private void SpawnGuidedStream(Vector3 target)
    {
        GuidedStream stream = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
        stream.SendTo(target);
        stream.setDamage(damage);
    }

    public void SetPrefab(GuidedStream prefab)
    {
        selectedPrefab = prefab;
    }
}