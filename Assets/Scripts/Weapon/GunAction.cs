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

    public NetworkVariable<int> netAmmo = new NetworkVariable<int>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone});
    [SerializeField] public int ammo = 0;

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
        if(reloading) return;

        aiming = true;

        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.Lerp(transform.localPosition, data.scopePos, data.fovSmooth * Time.deltaTime);

        owner.FpsCamera.fieldOfView = Mathf.Lerp(owner.FpsCamera.fieldOfView, data.scopeFov, data.fovSmooth * Time.deltaTime);
    }

    public void ReleaseAim()
    {
        //if(transform.localPosition == Vector3.zero && owner.FpsCamera.fieldOfView == data.defaultFov) return;
        
        aiming = false;

        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, data.resetSmooth * Time.deltaTime);
        owner.FpsCamera.fieldOfView = Mathf.Lerp(owner.FpsCamera.fieldOfView, data.defaultFov, data.resetSmooth * Time.deltaTime);
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

    public int GetAmmo() => netAmmo.Value;

    public void SetOwner(Player owner)
    {
        this.owner = owner;
    }

    public async void Fire()
    {
        if(owner == null || ammo <= 0 || reloading || shooting) return;
        shooting = true;
        netAmmo.Value--;
        transform.localPosition -= new Vector3(0, 0, data.kickbackForce);
        GunKickbackReset();
        FireServerRpc();
        
        await Task.Delay(1000 / data.shotsPerSecond);
        shooting = false;
    }

    public async void Reload()
    {
        if(owner == null || ammo >= data.maxAmmo || reloading) return;
        
        reloading = true;
        
        for(float time = 0; time <= data.reloadSpeed; time += Time.deltaTime)
        {
            ReleaseAim();
            await Task.Yield();
        }
        
        netAmmo.Value = data.maxAmmo;
        reloading = false;
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        if(Physics.Raycast(owner.CameraTransform.position, owner.CameraTransform.forward, out var hitInfo, data.range))
        {            
            var rb = hitInfo.transform.GetComponent<Rigidbody>();
            if(rb != null) rb.velocity += owner.CameraTransform.forward * data.hitForce;
            // Debug.Log(owner.CameraTransform.forward * data.hitForce);
        }
        FireClientRpc();
    }

    [ClientRpc]
    private void FireClientRpc()
    {
        // transform.localPosition -= new Vector3(0, 0, data.kickbackForce);
        // GunKickbackReset();
    }

    private async void GunKickbackReset()
    {
        if(aiming)
        {
            while(transform.localPosition != data.scopePos)
            {
                await Task.Yield();
                transform.localPosition = Vector3.Lerp(transform.localPosition, data.scopePos, data.resetSmooth * Time.deltaTime);
                if(transform.localPosition.z >= data.scopePos.z)
                {
                    transform.localPosition = data.scopePos;
                }
            }
        }
        else
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
}
