using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Security.Cryptography;

public class Move : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Base move speed in meters per second")]
    float BaseMoveSpeed = 5f;
    public float baseMoveSpeed
    {
        get
        {
            return BaseMoveSpeed;
        }
    }
    [SerializeField]
    [Tooltip("Base turn speed in degrees per second")]
    List<float> JumpSpeeds = new List<float> { 5 };

    int JumpCount = 0;
    public int jumpCount
    {
        get
        {
            return JumpCount;
        }
    }

    // Rigidbody PlayerRigidbody;
    PlayerInputActionV1 InputActions;
    BuffManager PlayerBuffManager;
    // JumpManager PlayerJumpManager;
    SkillManager PlayerSkillManager;

    CharacterController PlayerCharacterController;

    [SerializeField]
    float Gravity = 9.8f;
    [SerializeField]
    float HeightStillGround = 0.3f;

    [Header("Debug")]
    [SerializeField]
    Vector3 MoveVelocity;
    public Vector3 moveVelocity
    {
        get
        {
            return MoveVelocity;
        }
    }

    [SerializeField]
    bool IsGrounded;
    public bool isGrounded
    {
        get
        {
            return IsGrounded;
        }
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            InputActions = new PlayerInputActionV1();
            InputActions.Move.Enable();
            InputActions.Move.Jump.performed += OnJump;
            PlayerCharacterController = GetComponent<CharacterController>();
            PlayerBuffManager = GetComponent<BuffManager>();
            PlayerSkillManager = GetComponent<SkillManager>();
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (PlayerBuffManager.isStagger) return;
        if (JumpCount >= JumpSpeeds.Count) return;
        var jumpSpeed = JumpSpeeds[JumpCount];
        MoveVelocity.y = jumpSpeed;
        JumpCount++;
    }

    void InputMove()
    {
        var inputAxis = InputActions.Move.Move.ReadValue<Vector2>();
        if (inputAxis.magnitude > 0f)
        {
            var moveSpeed = (BaseMoveSpeed - PlayerBuffManager.slowDownSpeed) * (1 - PlayerBuffManager.slowDownPer);
            var inputVelocity = inputAxis * moveSpeed;
            MoveVelocity.x = inputVelocity.x;
            MoveVelocity.z = inputVelocity.y;
        }
    }

    void InputForward()
    {
        var inputForward = InputActions.Move.Look.ReadValue<Vector2>();
        if (inputForward.magnitude > 0f)
        {
            transform.forward = new Vector3(inputForward.x, 0, inputForward.y);
        }
        else
        {
            var inputPointPosition = InputActions.Move.PointPosition.ReadValue<Vector2>();
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
        if (PlayerCharacterController == null) return;
        MoveVelocity.y -= Gravity * Time.deltaTime;
        RaycastHit hit;
        var layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, HeightStillGround, layerMask))
        {
            if (MoveVelocity.y < 0)
                MoveVelocity.y = 0;
            JumpCount = 0;
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

    }

    // Update is called once per frame

    void Update()
    {
        if (!isLocalPlayer) return;
        MoveVelocity.x = 0;
        MoveVelocity.z = 0;
        if (PlayerSkillManager.canMove &&
            !PlayerBuffManager.isStagger)
        {
            InputMove();
            InputForward();
        }
        UseGravity();
        PlayerCharacterController.Move(MoveVelocity * Time.deltaTime);
    }

    void OnDestroy()
    {
        if (isLocalPlayer && InputActions != null)
        {
            InputActions.Move.Disable();
            InputActions.Dispose();
        }
    }
}
