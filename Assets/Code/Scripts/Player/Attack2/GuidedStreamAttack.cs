using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GuidedStreamAttack: NetworkBehaviour
{
    [SerializeField] GuidedStream selectedPrefab;
    [SerializeField] Camera camera;
    [SerializeField] float damage = 25;
    private CharacterClass playerClass;
    ulong playerID;

    private void Start()
    {
        playerClass = GetComponent<CharacterClass>();
        if (camera == null)
        {
            camera = Camera.main;
        }
        playerID = GetComponent<NetworkObject>().OwnerClientId;
    }

    public void Trigger()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        //int ignoreLayer = LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity)) //~ignoreLayer
        {
            SpawnGuidedStream(raycastHit.point);
        }
    }

    private void SpawnGuidedStream(Vector3 target)
    {
        if (!IsOwner) return; // Only the owner sends the request

        SpawnGuidedStreamServerRpc(target);
    }

    [ServerRpc]
    private void SpawnGuidedStreamServerRpc(Vector3 target, ServerRpcParams rpcParams = default)
    {
        GuidedStream stream = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
        
        NetworkObject networkObject = stream.GetComponent<NetworkObject>();
        networkObject.Spawn(); // Only the server can spawn it!
        stream.setPlayerID(playerID);
        stream.setDamage(damage);
        stream.SetPlayer(playerClass);

        // Send the target data to all clients
        stream.SendToClientRpc(target);
    }

    public void SetPrefab(GuidedStream prefab)
    {
        selectedPrefab = prefab;
    }
}