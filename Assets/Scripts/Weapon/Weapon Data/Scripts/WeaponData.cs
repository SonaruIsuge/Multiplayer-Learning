using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Gun,
    Utility,
    Melee,
}

public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponType type;

    [Space()]
    [Header("Pick and Throw")]

    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;
    public float animTime;

    [Space()]
    [Header("Damage")]    
    public int damage;
    
}
