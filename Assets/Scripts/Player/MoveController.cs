using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Security.Cryptography;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class MoveController : NetworkBehaviour
{
    ActorController m_ActorController;
    CharacterController m_CharacterController;
    PlayerInputAction m_InputActions;

    Vector3 m_Velocity;

    bool useGravity
    {
        get
        {
            return true;
        }
    }

    bool canMove
    {
        get
        {
            return true;
        }
    }

    void Start()
    {
        m_ActorController = GetComponent<ActorController>();
        if (isLocalPlayer)
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_InputActions = new PlayerInputAction();
            m_InputActions.Move.Enable();
            m_InputActions.Move.Jump.performed += OnJump;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
    }

    public void AddSpeed(Vector3 velocity)
    {
        m_Velocity += velocity;
    }

    void InputMove()
    {
        if(!canMove) return;
        
        var inputAxis = m_InputActions.Move.Move.ReadValue<Vector2>();
        var BaseMoveSpeed = m_ActorController.CharacterConfig.MoveSpeed;

        if (inputAxis.magnitude > 0f)
        {
            var moveSpeed = BaseMoveSpeed;
            var inputVelocity = inputAxis * moveSpeed;
        }
    }

    void UseGravity()
    {
        if (!useGravity) return;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            InputMove();
            UseGravity();
        }
    }

    void AfterUpdate()
    {
        if (isLocalPlayer)
        {
        }
    }
}