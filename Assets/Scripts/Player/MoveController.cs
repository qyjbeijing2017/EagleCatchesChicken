using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Security.Cryptography;
using UnityEditor.Animations;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(ActorController))]
[RequireComponent(typeof(SkillController))]
public class MoveController : NetworkBehaviour
{
    ActorController m_ActorController;
    CharacterController m_CharacterController;
    PlayerInputAction m_InputActions;
    Animator m_Animator;

    SkillController m_SkillController;

    Vector3 m_Velocity;

    int JumpCount = 0;
    public int jumpCount
    {
        get
        {
            return JumpCount;
        }
    }

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
            return !m_SkillController.skillRunning;
        }
    }

    void Start()
    {
        m_ActorController = GetComponent<ActorController>();
        m_CharacterController = GetComponent<CharacterController>();
        m_InputActions = new PlayerInputAction();
        m_Animator = GetComponentInChildren<Animator>();
        m_SkillController = GetComponent<SkillController>();
        m_InputActions.Move.Enable();
        m_InputActions.Move.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (JumpCount >= m_ActorController.CharacterConfig.JumpSpeeds.Count) return;
        var jumpSpeed = m_ActorController.CharacterConfig.JumpSpeeds[JumpCount];
        m_Velocity.y = jumpSpeed;
        JumpCount++;
    }

    public void AddSpeed(Vector3 velocity)
    {
        m_Velocity += velocity;
    }

    void InputMove()
    {
        if (!canMove) {
            m_Animator.SetBool("IsMove", false);
            return;
        }

        var inputAxis = m_InputActions.Move.Move.ReadValue<Vector2>();
        var BaseMoveSpeed = m_ActorController.CharacterConfig.MoveSpeed;

        var moveSpeed = BaseMoveSpeed;
        var inputVelocity = inputAxis * moveSpeed;
        m_Velocity.x = inputVelocity.x;
        m_Velocity.z = inputVelocity.y;

        if (inputVelocity.magnitude > 0f)
        {
            m_Animator.SetBool("IsMove", true);
        }
        else
        {
            m_Animator.SetBool("IsMove", false);
        }
    }

    void InputForward()
    {
        var inputForward = m_InputActions.Move.Look.ReadValue<Vector2>();
        if (inputForward.magnitude > 0f)
        {
            transform.forward = new Vector3(inputForward.x, 0, inputForward.y);
        }
        else
        {
            var inputPointPosition = m_InputActions.Move.PointPosition.ReadValue<Vector2>();
            var ray = Camera.main.ScreenPointToRay(inputPointPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var inputWorldPosition = hit.point;
                transform.forward = new Vector3(inputWorldPosition.x - transform.position.x, 0, inputWorldPosition.z - transform.position.z);
            }
        }
    }


    void UseGravity()
    {
        if (!useGravity) return;
        m_Velocity.y -= m_ActorController.PlayerController.GlobalConfig.Gravity * Time.deltaTime;

        RaycastHit hit;
        var layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, layerMask))
        {
            if (m_Velocity.y < 0)
                m_Velocity.y = 0;
            JumpCount = 0;
        }
    }

    void Update()
    {
        InputMove();
        InputForward();
        UseGravity();
        m_CharacterController.Move(m_Velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
    }

}