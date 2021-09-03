using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Melee Data", menuName ="Weapon/Melee")]
public class MeleeData : WeaponData
{
    public MeleeData()
    {
        type = WeaponType.Melee;
    }


}
