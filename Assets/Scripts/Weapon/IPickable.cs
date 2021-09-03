using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable 
{
    void PickUp(ulong ownerNetId);
    void Drop();

    void SetOwner(Player player);
}
