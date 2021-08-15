using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public class PlayerMovement : PlayerComponent
{
    CharacterProps props = null;

    Vector2 movement = Vector2.zero;
    bool jump = false;

    bool isGrounded = true;
    Vector3 velocity = Vector3.zero;

    public PlayerMovement(Player player) : base(player)
    {
        this.props = player.Props;        
    }

    public override void OnEnable()
    {
        Controller.GamePlay.Movement.performed += OnMovementPerformed;
        Controller.GamePlay.Movement.canceled += OnMovementCanceled;
        Controller.GamePlay.Jump.performed += OnJumpPerformed;
        Controller.GamePlay.Jump.canceled += OnJumpCanceled;
    }

    public override void OnDisable()
    {
        Controller.GamePlay.Movement.performed -= OnMovementPerformed;
        Controller.GamePlay.Movement.canceled -= OnMovementCanceled;
        Controller.GamePlay.Jump.performed -= OnJumpPerformed;
        Controller.GamePlay.Jump.canceled -= OnJumpCanceled;
    }

    public override void Tick()
    {
        HandleGravity();
        Jump();
        Move();
    }

    void OnMovementPerformed(InputAction.CallbackContext ctx) => movement = ctx.ReadValue<Vector2>();
    void OnMovementCanceled(InputAction.CallbackContext ctx) => movement = Vector2.zero;
    void OnJumpPerformed(InputAction.CallbackContext ctx) => jump = true;
    void OnJumpCanceled(InputAction.CallbackContext ctx) => jump = false;
    
    private void Move()
    {
        Vector3 move = Player.transform.forward * movement.y + Player.transform.right * movement.x;
        Player.CharacterController.Move(move * props.MoveSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        isGrounded = Physics.CheckSphere(Player.GroundCheck.position, 0.4f, LayerMask.GetMask("Ground"));

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        // v = g * t * t
        velocity.y += WorldNumber.GRAVITY * Time.deltaTime;
        Player.CharacterController.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if(jump && isGrounded)
        {
            // v = √ ( h * -2 * g )
            velocity.y = Mathf.Sqrt(props.JumpHeight * -2f * WorldNumber.GRAVITY);
        }
    }
}
