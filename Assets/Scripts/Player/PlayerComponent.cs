using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerComponent
{
    Player player = null;
    InputController inputController = null;

    public Player Player => player;
    public InputController Controller => inputController;

    public PlayerComponent(Player player)
    {
        this.player = player;
        this.inputController = player.Controller;
    }

    public virtual void OnEnable() {}
    public virtual void OnDisable() {}
    public virtual void Tick() {}

}
