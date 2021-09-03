using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using System.Threading.Tasks;

public class PlayerEquipment : NetworkBehaviour
{
    [SerializeField] bool interact = false;
    [SerializeField] bool drop = false;
    [SerializeField] bool reload = false;
    [SerializeField] bool fireDown = false;
    [SerializeField] bool fireHold = false;
    [SerializeField] bool fireUp = false;
    [SerializeField] bool aim = false;

    [SerializeField] Player player = null;
    [SerializeField] InputController Controller = null;
    [SerializeField] CharacterProps props = null;

    public GameObject TestWeapon = null;    
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
        Controller.GamePlay.Reload.performed += OnReloadPerformed;
        Controller.GamePlay.Reload.canceled += OnReloadCanceled;
        Controller.GamePlay.Aim.performed += OnAimPerformed;
        Controller.GamePlay.Aim.canceled += OnAimCanceled;
        Controller.GamePlay.Fire.performed += OnFirePerformed;
        Controller.GamePlay.Fire.canceled += OnFireCanceled;
    }

    public void OnDisable()
    {
        Controller.GamePlay.Interact.performed -= OnInteractPerformed;
        Controller.GamePlay.Interact.canceled -= OnInteractCanceled;
        Controller.GamePlay.Drop.performed -= OnDropPerformed;
        Controller.GamePlay.Drop.canceled -= OnDropCanceled;
        Controller.GamePlay.Reload.performed -= OnReloadPerformed;
        Controller.GamePlay.Reload.canceled -= OnReloadCanceled;
        Controller.GamePlay.Aim.performed -= OnAimPerformed;
        Controller.GamePlay.Aim.canceled -= OnAimCanceled;  
        Controller.GamePlay.Fire.performed -= OnFirePerformed;
        Controller.GamePlay.Fire.canceled -= OnFireCanceled;
    }
    void OnInteractPerformed(InputAction.CallbackContext ctx) => interact = true;
    void OnInteractCanceled(InputAction.CallbackContext ctx) => interact = false;
    void OnDropPerformed(InputAction.CallbackContext ctx) => drop = true;
    void OnDropCanceled(InputAction.CallbackContext ctx) => drop = false;
    void OnReloadPerformed(InputAction.CallbackContext ctx) => reload = true;
    void OnReloadCanceled(InputAction.CallbackContext ctx) => reload = false;
    void OnAimPerformed(InputAction.CallbackContext ctx) => aim = true;
    void OnAimCanceled(InputAction.CallbackContext ctx)=> aim = false;
    void OnFirePerformed(InputAction.CallbackContext ctx)
    {
        fireHold = true;
        var button = ctx.control as ButtonControl;
        if(button.wasPressedThisFrame)
        {
            FirePress();
        }
    }

    void OnFireCanceled(InputAction.CallbackContext ctx)
    {
        fireHold = false;
        var button = ctx.control as ButtonControl;
        if(button.wasReleasedThisFrame)
        {
            FireRelease();
        }
    }

    private async void FirePress()
    {
        fireDown = true;
        // Debug.Log(fireDown + "frame" + Time.frameCount);
        await Task.Yield();
        fireDown = false;
        // Debug.Log(fireDown + "frame" + Time.frameCount);
    }
    private async void FireRelease()
    {
        fireUp = true;
        // Debug.Log(fireUp + "frame" + Time.frameCount);
        await Task.Yield();
        fireUp = false;
        // Debug.Log(fireUp + "frame" + Time.frameCount);
    }

    void Update()
    {
        if(IsLocalPlayer)
        {
            if(interact)
            {
                TryGetItem();

                // ServerRpc 可能運行橫跨多幀
                interact = false;
            }

            if(drop)
            {
                TryDrop();

                drop = false;
            }

            if(aim)
            {
                Debug.Log("aim");
                // WeaponAction.Aim();
            }

            if(reload)
            {
                if(!equipItem) return;

                Debug.Log("reload");
                equipItem.GetComponent<IWeaponAction>().Reload();
            }

            if(fireDown)
            {
                if(!equipItem) return;

                // Debug.Log("fire down");
                equipItem.GetComponent<IWeaponAction>().FireDown();
            }

            if(fireHold)
            {
                if(!equipItem) return;

                // Debug.Log("fire hold");
                equipItem.GetComponent<IWeaponAction>().FireHold();
            }

            if(fireUp)
            {
                if(!equipItem) return;

                // Debug.Log("fire up");
                equipItem.GetComponent<IWeaponAction>().FireUp();
            }
        }        
    }   

    private void TryGetItem()
    {
        RaycastHit hit;
        if(Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out hit, 30f))
        {
            Debug.Log(hit.transform.name);
            if(hit.transform.GetComponent<Weapon>())
            {
                // PickUpServerRpc(NetworkManager.Singleton.LocalClientId, hit.transform.GetComponent<NetworkObject>().NetworkObjectId);
                if(equipItem != null) TryDrop();

                // equipItem = hit.transform.gameObject;
                SetPickUpServerRpc(hit.transform.GetComponent<NetworkObject>().NetworkObjectId);
                hit.transform.GetComponent<Weapon>().PickUp(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    private void TryDrop()
    {
        if(equipItem == null) return;
        // DropServerRpc(); 
        equipItem.GetComponent<Weapon>().Drop();
        equipItem = null;       
    }

    [ServerRpc]
    private void SetPickUpServerRpc(ulong itemNetId)
    {
        SetPickUpClientRpc(itemNetId);
    }

    [ClientRpc]
    private void SetPickUpClientRpc(ulong itemNetId)
    {
        equipItem = NetworkSpawnManager.SpawnedObjects[itemNetId].gameObject;
        equipItem.GetComponent<Weapon>().SetOwner(this.player);
    }

#region Not Using function

    // [ServerRpc]
    // public void PickUpServerRpc(ulong netId, ulong itemId)
    // {
    //     Debug.Log("pick server");

    //     NetworkSpawnManager.SpawnedObjects[itemId].ChangeOwnership(netId);
    //     PickUpClientRpc(itemId);
    // }

    // [ClientRpc]
    // public void PickUpClientRpc(ulong itemNetId)
    // {
    //     Debug.Log("pick client");

    //     NetworkObject netObj = NetworkSpawnManager.SpawnedObjects[itemNetId];
    //     equipItem = netObj.gameObject;
    //     Rigidbody rb = equipItem.GetComponent<Rigidbody>();
    //     rb.isKinematic = true;
    //     rb.useGravity = false;
    //     equipItem.transform.SetParent(player.WeaponHolder);
    //     equipItem.transform.localPosition = Vector3.zero;
    //     equipItem.transform.localRotation = Quaternion.identity;
    // }

    // [ServerRpc]
    // private void DropServerRpc()
    // {
    //     Debug.Log("drop server");

    //     equipItem.GetComponent<NetworkObject>().RemoveOwnership();
    //     DropClientRpc();
    // }

    // [ClientRpc]
    // private void DropClientRpc()
    // {
    //     Debug.Log("drop client");

    //     Rigidbody rb = equipItem.GetComponent<Rigidbody>();
    //     rb.isKinematic = false;
    //     rb.useGravity = true;
    //     equipItem.transform.SetParent(null);
    //     equipItem = null;
    // }

    // [ServerRpc]
    // public void PickUpServerRpc(ulong netId)
    // {
    //     if(equipItem != null) return;
        
    //     Debug.Log("pick server");

    //     GameObject go = Instantiate(TestWeapon);
    //     go.GetComponent<NetworkObject>().SpawnWithOwnership(netId);
    //     ulong itemNetId = go.GetComponent<NetworkObject>().NetworkObjectId;

    //     PickUpClientRpc(itemNetId);
    // }
    
    // [ServerRpc]
    // private void PickServerRpc(ulong itemNetId)
    // {
    //     NetworkObject no = NetworkSpawnManager.SpawnedObjects[itemNetId];
    //     NetworkManager.Destroy(no.gameObject);
    // }
#endregion

}