using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : PlayerComponent
{
    CharacterProps props = null;

    Vector2 rotation = Vector2.zero;

    float mouseX = 0f;
    float mouseY = 0f;
    float xRotation = 0f;

    public PlayerLook(Player player) : base(player)
    {
        props = player.Props;
    }

    public override void OnEnable()
    {
        Controller.GamePlay.Rotate.performed += OnRotationPerformed;
        Controller.GamePlay.Rotate.canceled += OnRotationCanceled;
    }

    public override void OnDisable()
    {
        Controller.GamePlay.Rotate.performed -= OnRotationPerformed;
        Controller.GamePlay.Rotate.canceled -= OnRotationCanceled;
    }

    public override void Tick()
    {
        Rotate();
    }

    void OnRotationPerformed(InputAction.CallbackContext ctx) => rotation = ctx.ReadValue<Vector2>();
    void OnRotationCanceled(InputAction.CallbackContext ctx) => rotation = Vector2.zero;

    void Rotate()
    {
        mouseX = rotation.x * props.MouseSensitivity * Time.deltaTime;
        mouseY = rotation.y * props.MouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, props.minLook, props.maxLook);

        Player.transform.Rotate(Vector3.up * mouseX);
        Player.CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
