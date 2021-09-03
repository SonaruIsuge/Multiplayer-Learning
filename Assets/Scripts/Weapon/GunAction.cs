using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Threading.Tasks;

public class GunAction : NetworkBehaviour, IWeaponAction
{
    [SerializeField] private Player owner = null;
    [SerializeField] private GunData data = null;

    [SerializeField] private bool shooting = false;
    [SerializeField] private bool reloading = false;
    [SerializeField] private bool aiming = false;

    NetworkVariable<int> netAmmo = new NetworkVariable<int>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
    [SerializeField] private int ammo = 0;

    void Awake()
    {
        netAmmo.Value = data.maxAmmo;
        ammo = netAmmo.Value;
    }

    void Update()
    {
        ammo = netAmmo.Value;
    }

    public void Aim()
    {

    }

    public void FireDown()
    {
        if(!data.tapable) return;
        Fire();
    }

    public void FireHold()
    {
        if(data.tapable) return;
        Fire();
    }

    public void FireUp()
    {

    }
    
    public void SetOwner(Player owner)
    {
        this.owner = owner;
    }

    public async void Fire()
    {
        if(owner == null || ammo <= 0 || reloading || shooting) return;
        shooting = true;
        netAmmo.Value--;
        FireServerRpc();
        
        await Task.Delay(1000 / data.shotsPerSecond);
        shooting = false;
    }

    public async void Reload()
    {
        if(owner == null || ammo >= data.maxAmmo || reloading) return;
        reloading = true;
        
        await Task.Delay((int)(data.reloadSpeed * 1000));
        netAmmo.Value = data.maxAmmo;
        reloading = false;
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        FireClientRpc();
    }

    [ClientRpc]
    private void FireClientRpc()
    {
        transform.localPosition -= new Vector3(0, 0, data.kickbackForce);
        if(Physics.Raycast(owner.CameraTransform.position, owner.CameraTransform.forward, out var hitInfo, data.range))
        {            
            var rb = hitInfo.transform.GetComponent<Rigidbody>();
            if(rb != null) rb.velocity += owner.CameraTransform.forward * data.hitForce;
            // Debug.Log(owner.CameraTransform.forward * data.hitForce);
        }
        GunKickbackReset();
    }

    private async void GunKickbackReset()
    {
        while(transform.localPosition != Vector3.zero)
        {
            await Task.Yield();
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, data.resetSmooth * Time.deltaTime);
            if(transform.localPosition.z >= 0)
            {
                transform.localPosition = Vector3.zero;
            }
        }
    }
}
