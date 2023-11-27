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
    PlayerSkill m_PlayerSkill;
    [SyncVar]
    Vector3 m_Velocity;
    [SyncVar]
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

    public bool isFreezeInput =>
    m_PlayerSkill.isKnockedBack ||
    m_PlayerSkill.isForceMoving ||
    m_PlayerHealth.isDead ||
    m_PlayerBuff.beStunning ||
    m_PlayerHealth.isKnockedBack ||
    m_PlayerHealth.isKnockedOff;

    public event Action onJump;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_PlayerBuff = GetComponent<PlayerBuff>();
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_PlayerSkill = GetComponent<PlayerSkill>();
        if (isLocalPlayer)
        {
            m_InputActions = new PlayerInputAction();
            m_InputActions.Move.Enable();
            m_InputActions.Move.Jump.performed += OnInputJump;
        }
    }

    void OnInputJump(InputAction.CallbackContext context)
    {
        if (isFreezeInput) return;
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
        if (isFreezeInput) return;

        var inputAxis = m_InputActions.Move.Move.ReadValue<Vector2>();

        var moveSpeed = playerConfig.MoveSpeed * m_PlayerBuff.speedMultiplier + m_PlayerBuff.speedAddition;
        var inputVelocity = inputAxis * moveSpeed;
        m_InputVelocity.x = inputVelocity.x;
        m_InputVelocity.z = inputVelocity.y;
    }

    void InputDirect()
    {
        if (isFreezeInput) return;
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
        if (!m_CharacterController.isGrounded)
        {
            m_Velocity.y -= globalConfig.Gravity * Time.deltaTime;
            isGrounded = false;
        }
        else
        {
            m_Velocity.y -= 0f;
            isGrounded = true;
            jumpCount = 0;
        }
    }


    void Update()
    {

        if (isLocalPlayer)
        {
            InputMove();
            InputDirect();
        }
        if (isOwned || isServer && identity == PlayerIdentity.Dummy)
        {
            UseGravity();
        }
    }

    void LateUpdate()
    {
        if (isOwned || isServer && identity == PlayerIdentity.Dummy)
        {
            m_CharacterController.Move(m_Velocity * Time.deltaTime + m_MovePosition + m_InputVelocity * Time.deltaTime);
            m_MovePosition = Vector3.zero;
            m_InputVelocity = Vector3.zero;
        }
    }

}