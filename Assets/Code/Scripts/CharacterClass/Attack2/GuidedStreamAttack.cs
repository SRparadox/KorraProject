using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GuidedStreamAttack: MonoBehaviour
{
    [SerializeField] GuidedStream streamPrefab;
    [SerializeField] Camera cam;

    public void Trigger()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
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
        GuidedStream stream = Instantiate(streamPrefab, transform.position, Quaternion.identity);
        stream.SendTo(target);
    }
}
