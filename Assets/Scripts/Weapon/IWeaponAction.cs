using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponAction 
{ 
   void SetOwner(Player owner);
   void Aim();
   void ReleaseAim();
   void Reload();
   void FireDown();
   void FireHold();
   void FireUp();
}
