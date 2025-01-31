using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedStreamAttack: MonoBehaviour
{
    [SerializeField] GuidedStream streamPrefab;

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
        GuidedStream stream = Instantiate(streamPrefab, transform.position, Quaternion.identity);
        stream.SendTo(target);
    }
}
