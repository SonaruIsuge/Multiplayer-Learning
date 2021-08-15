using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="new character", menuName ="Character")]
public class CharacterProps : ScriptableObject
{
    public string CharacterName;

    [Space()]
    [Header("Movement")]

    public float MoveSpeed;
    public float MouseSensitivity;
    public float JumpHeight;

    public float maxLook;
    public float minLook;

    [Space()]
    [Header("Shooting")]

    //撿武器距離
    public float pickupRange;
    public float pickupRadius;

    //槍口跟隨準心晃動
    public float swaySize;
    public float swaySmooth;

    [Space()]
    [Header("WeaponManagerment")]
    public int maxEquipment;
}
