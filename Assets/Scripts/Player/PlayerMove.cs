using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using System;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(PlayerBuff))]
public class PlayerMove : PlayerComponent
{
    CharacterController m_CharacterController;
    PlayerInputAction m_InputActions;
    PlayerHealth m_PlayerHealth;
    PlayerBuff m_PlayerBuff;
    Vector3 m_Velocity;
    Vector3 m_MovePosition;
    Vector3 m_InputVelocity;

    public Vector3 inputVelocity
    {
        get
        {
            return m_InputVelocity;
        }
    }

    public Vector3 moveVelocity
    {
        get
        {
            return m_Velocity;
        }
    }

    public Vector3 movePosition
    {
        get
        {
            return m_MovePosition;
        }
    }

    public int jumpCount { get; private set; }

    public bool isGrounded { get; private set; }

    public event Action onJump;
    public event Action onGrounded;

    void Start()
    {
        if (isLocalPlayer)
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_PlayerBuff = GetComponent<PlayerBuff>();
            m_PlayerHealth = GetComponent<PlayerHealth>();
            m_InputActions = new PlayerInputAction();
            m_InputActions.Move.Enable();
            m_InputActions.Move.Jump.performed += OnJump;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (m_PlayerBuff.beStunning) return;
        if (m_PlayerHealth.isDead) return;
        if (m_PlayerHealth.isKnockedBack) return;
        if (m_PlayerHealth.isKnockedOff) return;
        if (jumpCount >= playerConfig.JumpSpeeds.Count) return;
        var jumpSpeed = playerConfig.JumpSpeeds[jumpCount];
        m_Velocity.y = jumpSpeed;
        jumpCount++;
        onJump?.Invoke();
    }

    public void AddVelocity(Vector3 velocity)
    {
        m_Velocity += velocity;
    }

    public void AddMovePosition(Vector3 movePosition)
    {
        m_MovePosition += movePosition;
    }

    void InputMove()
    {
        if (m_PlayerHealth.isDead) return;
        if (m_PlayerHealth.isKnockedBack) return;
        if (m_PlayerHealth.isKnockedOff) return;
        if (m_PlayerBuff.beStunning) return;

        var inputAxis = m_InputActions.Move.Move.ReadValue<Vector2>();

        var moveSpeed = playerConfig.MoveSpeed * m_PlayerBuff.speedMultiplier + m_PlayerBuff.speedAddition;
        var inputVelocity = inputAxis * moveSpeed;
        m_InputVelocity.x = inputVelocity.x * Time.deltaTime;
        m_InputVelocity.z = inputVelocity.y * Time.deltaTime;
        m_MovePosition.x += inputVelocity.x * Time.deltaTime;
        m_MovePosition.z += inputVelocity.y * Time.deltaTime;
    }

    void InputDirect()
    {
        if (m_PlayerHealth.isDead) return;
        if (m_PlayerHealth.isKnockedBack) return;
        if (m_PlayerHealth.isKnockedOff) return;
        if (m_PlayerBuff.beStunning) return;
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
        m_Velocity.y -= globalConfig.Gravity * Time.deltaTime;

        RaycastHit hit;
        var layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, layerMask))
        {
            if (m_Velocity.y < 0)
                m_Velocity.y = 0;
            jumpCount = 0;
            if (!isGrounded)
            {
                isGrounded = true;
                onGrounded?.Invoke();
            }

        }
        else
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        InputMove();
        InputDirect();
        UseGravity();
        m_CharacterController.Move(m_Velocity * Time.deltaTime + m_MovePosition);
        m_Velocity = m_CharacterController.velocity - m_MovePosition / Time.deltaTime;
        var speed = m_Velocity.magnitude - globalConfig.Drag * Time.deltaTime;
        if (speed < 0) speed = 0;
        m_Velocity = m_Velocity.normalized * speed;
        m_MovePosition = Vector3.zero;
        m_InputVelocity = Vector3.zero;
    }

}