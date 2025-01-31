using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedStream: MonoBehaviour
{
    [SerializeField] GuidedStreamControl streamPrefab;

    public void Trigger()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            SpawnGuidedStream(hit.point);
        }
    }

    private void SpawnGuidedStream(Vector3 target)
    {
        GuidedStreamControl stream = Instantiate(streamPrefab, transform.position, Quaternion.identity);
        stream.Spawn();
    }
}
