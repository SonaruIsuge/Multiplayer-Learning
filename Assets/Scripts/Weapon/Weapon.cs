using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;

[RequireComponent(typeof(NetworkObject))]
public class Weapon : NetworkBehaviour, IPickable
{

    private Rigidbody rb = null;
    [SerializeField] private Player owner = null;
    [SerializeField] public IWeaponAction weaponAction = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weaponAction = GetComponent<IWeaponAction>();
    }

    public void PickUp(ulong ownerNetId)
    {
        PickUpServerRpc(ownerNetId);
    }

    public void Drop()
    {
        DropServerRpc();
    }

    public void SetOwner(Player player)
    {
        this.owner = player;
        weaponAction.SetOwner(owner);

        transform.SetParent(owner.WeaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpServerRpc(ulong ownerNetId)
    {
        ulong itemId = this.GetComponent<NetworkObject>().NetworkObjectId;
        this.GetComponent<NetworkObject>().ChangeOwnership(ownerNetId);
                
        PickUpClientRpc(itemId, ownerNetId);
    }

    [ClientRpc]
    private void PickUpClientRpc(ulong itemId, ulong ownerNetId)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        // owner = NetworkManager.Singleton.ConnectedClients[ownerNetId].PlayerObject.GetComponent<Player>();
        // weaponAction.SetOwner(owner);

        // transform.SetParent(owner.WeaponHolder);
        // transform.localPosition = Vector3.zero;
        // transform.localRotation = Quaternion.identity;
    }

    [ServerRpc]
    private void DropServerRpc()
    {
        this.GetComponent<NetworkObject>().RemoveOwnership();
        DropClientRpc();
    }

    [ClientRpc]
    private void DropClientRpc()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        this.transform.SetParent(null);

        owner = null;
        weaponAction.SetOwner(null);
    }
}
