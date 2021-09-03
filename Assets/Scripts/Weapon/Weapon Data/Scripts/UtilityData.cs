using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Utility Data", menuName ="Weapon/Utility")]
public class UtilityData : WeaponData
{
    public UtilityData()
    {
        type = WeaponType.Utility;
    }

    [Space()]
    [Header("Throw Attack")]

    public float forwardForce;
    public float UpwardForce;

    [Space()]
    [Header("Attack")]

    public float delayTime;
    public float explodeRadius;
    public float explodeForce;
}
