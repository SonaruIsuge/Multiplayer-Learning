using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;

public class PlayerEquipment : NetworkBehaviour
{
    [SerializeField]bool interact = false;
    [SerializeField]bool drop = false;

    [SerializeField] Player player = null;
    [SerializeField] InputController Controller = null;
    [SerializeField] CharacterProps props = null;

    public GameObject TestWeapon = null;    
    [SerializeField] bool canEquip = true;
    [SerializeField] GameObject equipItem = null;

    void Awake()
    {
        player = GetComponent<Player>();
        Controller = player.Controller;
        props = player.Props;
    }
    

    public void OnEnable()
    {
        Controller.GamePlay.Interact.performed += OnInteractPerformed;
        Controller.GamePlay.Interact.canceled += OnInteractCanceled;
        Controller.GamePlay.Drop.performed += OnDropPerformed;
        Controller.GamePlay.Drop.canceled += OnDropCanceled;
    }

    public void OnDisable()
    {
        Controller.GamePlay.Interact.performed -= OnInteractPerformed;
        Controller.GamePlay.Interact.canceled -= OnInteractCanceled;
        Controller.GamePlay.Drop.performed -= OnDropPerformed;
        Controller.GamePlay.Drop.canceled -= OnDropCanceled;
    }
    void OnInteractPerformed(InputAction.CallbackContext ctx) => interact = true;
    void OnInteractCanceled(InputAction.CallbackContext ctx) => interact = false;
    void OnDropPerformed(InputAction.CallbackContext ctx) => drop = true;
    void OnDropCanceled(InputAction.CallbackContext ctx) => drop = false;

    void Update()
    {
        if(IsLocalPlayer)
        {
            if(interact)
            {
                // PickUpServerRpc(NetworkManager.Singleton.LocalClientId);
                TryGetItem();

                // ServerRpc 可能運行橫跨多幀
                interact = false;
            }

            if(drop)
            {
                TryDrop();

                drop = false;
            }

        }
        
    }

    
    [ServerRpc]
    public void PickUpServerRpc(ulong netId)
    {
        if(equipItem != null) return;
        
        Debug.Log("pick server");

        GameObject go = Instantiate(TestWeapon);
        go.GetComponent<NetworkObject>().SpawnWithOwnership(netId);
        ulong itemNetId = go.GetComponent<NetworkObject>().NetworkObjectId;

        PickUpClientRpc(itemNetId);
    }

    [ClientRpc]
    public void PickUpClientRpc(ulong itemNetId)
    {
        
        Debug.Log("pick client");

        NetworkObject netObj = NetworkSpawnManager.SpawnedObjects[itemNetId];
        equipItem = netObj.gameObject;
        Rigidbody rb = equipItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        equipItem.transform.SetParent(player.WeaponHolder);
        equipItem.transform.localPosition = Vector3.zero;
        equipItem.transform.localRotation = Quaternion.identity;

        canEquip = false;
    }

    private void TryGetItem()
    {
        RaycastHit hit;
        if(Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out hit, 30f))
        {
            Debug.Log(hit.transform.name);
            if(hit.transform.GetComponent<Item>())
            {
                canEquip = true;
                PickServerRpc(hit.transform.GetComponent<NetworkObject>().NetworkObjectId);
                PickUpServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    private void TryDrop()
    {
        if(equipItem == null) return;
        DropServerRpc();
    }

    [ServerRpc]
    private void PickServerRpc(ulong itemNetId)
    {
        NetworkObject no = NetworkSpawnManager.SpawnedObjects[itemNetId];
        NetworkManager.Destroy(no.gameObject);
    }

    [ServerRpc]
    private void DropServerRpc()
    {
        Debug.Log("drop server");

        equipItem.GetComponent<NetworkObject>().RemoveOwnership();
        DropClientRpc();
    }

    [ClientRpc]
    private void DropClientRpc()
    {
        Debug.Log("drop client");

        Rigidbody rb = equipItem.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        equipItem.transform.SetParent(null);
        equipItem = null;
    }
}
