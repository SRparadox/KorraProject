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
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Player"))
            {
                SpawnGuidedStream(hit.point);
                break;
            }
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
