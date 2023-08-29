using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Move : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Base move speed in meters per second")]
    float BaseMoveSpeed = 10f;
    [SerializeField]
    [Tooltip("Base turn speed in degrees per second")]
    List<float> JumpSpeeds = new List<float> { 5 };

    int JumpCount = 0;
    public int jumpCount{
        get {
            return JumpCount;
        }
    }
    Vector2 MoveVelocity;
    public Vector2 moveVelocity {
        get {
            return MoveVelocity;
        }
    }

    Rigidbody PlayerRigidbody;
    PlayerInputAction InputActions;
    BuffManager PlayerBuffManager;
    JumpManager PlayerJumpManager;
    SkillManager PlayerSkillManager;

    void Start()
    {
        if(isLocalPlayer) {
            InputActions = new PlayerInputAction();
            InputActions.Move.Enable();
            InputActions.Move.Jump.performed += OnJump;

            PlayerRigidbody = GetComponent<Rigidbody>();
            PlayerBuffManager = GetComponent<BuffManager>();
            PlayerJumpManager = GetComponentInChildren<JumpManager>();
            PlayerSkillManager = GetComponent<SkillManager>();
            PlayerJumpManager.onGrounded += () => {
                JumpCount = 0;
            };
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if(PlayerBuffManager.isStagger) return;
        if (JumpCount >= JumpSpeeds.Count) return;
        var jumpSpeed = JumpSpeeds[JumpCount];
        PlayerRigidbody.velocity = Vector3.up * jumpSpeed;
        JumpCount++;
    }


    // Update is called once per frame

    void Update()
    {
        if (isLocalPlayer)
        {
            if(PlayerBuffManager.isStagger) return;

            MoveVelocity = Vector2.zero;
            
            var inputAxis = InputActions.Move.Move.ReadValue<Vector2>();
            if (inputAxis.magnitude > 0f)
            {
                var moveSpeed = (BaseMoveSpeed - PlayerBuffManager.slowDownSpeed) * (1 - PlayerBuffManager.slowDownPer);
                MoveVelocity = inputAxis * moveSpeed;

                var deltaTransform = new Vector3(MoveVelocity.x, 0, MoveVelocity.y) * Time.deltaTime;
                var newPosition = transform.position + deltaTransform;
                transform.position = newPosition;
            }
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
                if (Physics.Raycast(ray, out hit)) {
                    var inputWorldPosition = hit.point;
                    transform.forward = new Vector3(inputWorldPosition.x - transform.position.x, 0, inputWorldPosition.z - transform.position.z);
                }

            }
        }
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
