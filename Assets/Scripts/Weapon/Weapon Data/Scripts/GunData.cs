using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Gun Data", menuName ="Weapon/Gun")]
public class GunData : WeaponData
{
    public GunData()
    {
        type = WeaponType.Gun;
    }

    [Space()]
    [Header("Fire")]

    public int maxAmmo;
    public int shotsPerSecond;
    public float reloadSpeed;
    public float hitForce;
    public float range;
    public bool tapable;
    public float kickbackForce;
    public float resetSmooth;

    [Space()]
    [Header("Scope")]

    public Vector3 scopePos;
    public float defaultFov;
    public float scopeFov;
    public float fovSmooth;    
    public Sprite crossHair;
    public Sprite scopeCrosshair;

}
