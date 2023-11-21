using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerMove : PlayerComponent
{
    CharacterController m_CharacterController;
    PlayerInputAction m_InputActions;
    Vector3 m_Velocity;

    public Vector3 moveVelocity
    {
        get
        {
            return m_Velocity;
        }
    }

    int m_JumpCount = 0;

    void Start()
    {
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
        if (m_JumpCount >= character.JumpSpeeds.Count) return;
        var jumpSpeed = character.JumpSpeeds[m_JumpCount];
        m_Velocity.y = jumpSpeed;
        m_JumpCount++;
    }

    public void AddSpeed(Vector3 velocity)
    {
        m_Velocity += velocity;
    }

    void InputMove()
    {
        var inputAxis = m_InputActions.Move.Move.ReadValue<Vector2>();

        var moveSpeed = character.MoveSpeed;
        var inputVelocity = inputAxis * moveSpeed;
        m_Velocity.x = inputVelocity.x;
        m_Velocity.z = inputVelocity.y;
    }

    void InputDirect()
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
        m_Velocity.y -= global.Gravity * Time.deltaTime;

        RaycastHit hit;
        var layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, layerMask))
        {
            if (m_Velocity.y < 0)
                m_Velocity.y = 0;
            m_JumpCount = 0;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        InputMove();
        InputDirect();
        UseGravity();
        m_CharacterController.Move(m_Velocity * Time.deltaTime);
    }

}