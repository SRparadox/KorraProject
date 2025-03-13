using Unity.Netcode;
using UnityEngine;

public class GuidedStreamAttack : NetworkBehaviour
{
    [SerializeField] GuidedStream selectedPrefab;
    [SerializeField] Camera camera;
    [SerializeField] float damage = 25f;

    private void Start()
    {
        if (camera == null)
            camera = Camera.main;
    }

    public void Trigger()
    {
        if (IsOwner)
            TriggerServerRpc();
    }

    [ServerRpc]
    private void TriggerServerRpc()
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
        var netObj = stream.GetComponent<NetworkObject>();
        if (netObj != null)
            netObj.Spawn();
        stream.SendTo(target);
        stream.setDamage(damage);
        stream.SetPlayer(GetComponent<CharacterClass>());
    }

    public void SetPrefab(GuidedStream prefab)
    {
        selectedPrefab = prefab;
    }
}
