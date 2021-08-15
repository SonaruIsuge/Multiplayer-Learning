using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Player : NetworkBehaviour
{
    public CharacterProps Props = null;
    public CharacterController CharacterController = null;
    public InputController Controller;

    [SerializeField] List<PlayerComponent> components = null;

    public Transform GroundCheck = null;
    public Transform CameraTransform = null;
    
    void Awake()
    {

        Controller = new InputController();
        CharacterController = this.GetComponent<CharacterController>();
        components = new List<PlayerComponent>();
        CameraTransform = GetComponentInChildren<Camera>().transform;

        components.Add(new PlayerMovement(this));
        components.Add(new PlayerLook(this));
    }

    void OnEnable()
    {
        Controller.GamePlay.Enable();        
        foreach(var c in components) c.OnEnable();        
    }

    void OnDisable()
    {
        foreach(var c in components) c.OnDisable();        
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!IsLocalPlayer)
        {
            CameraTransform.GetComponent<Camera>().enabled = false;
            CameraTransform.GetComponent<AudioListener>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer)
        {
            foreach(var c in components) c.Tick();
        }
    }
}


[System.Serializable]
public static class WorldNumber
{
    public static float GRAVITY = -19.6f;
}