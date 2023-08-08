using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Move : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Base move speed in meters per second")]
    private float BaseMoveSpeed = 1f;
    [SerializeField]
    [Tooltip("Base turn speed in degrees per second")]
    private List<float> JumpSpeeds = new List<float> { 5 };
    private int JumpCount = 0;
    // Start is called before the first frame update


    private Rigidbody rigidbody;
    private PlayerInputAction InputActions;
    void Start()
    {
        if(isLocalPlayer) {
            InputActions = new PlayerInputAction();
            InputActions.Player.Enable();
            InputActions.Player.Jump.performed += OnJump;

            rigidbody = GetComponent<Rigidbody>();
        }
    }

    

    void OnCollisionEnter(Collision collision)
    {
       if(isLocalPlayer) {
        if (collision.gameObject.tag == "Ground")
            JumpCount = 0;
       }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (JumpCount >= JumpSpeeds.Count) return;
        var jumpSpeed = JumpSpeeds[JumpCount];
        rigidbody.velocity = Vector3.up * jumpSpeed;
        JumpCount++;
    }


    // Update is called once per frame

    void Update()
    {
        if (isLocalPlayer)
        {
            var inputAxis = InputActions.Player.Move.ReadValue<Vector2>();
            if (inputAxis.magnitude > 0f)
            {
                var moveSpeed = BaseMoveSpeed * Time.deltaTime;
                var moveVector = inputAxis * moveSpeed;
                var newPosition = transform.position + new Vector3(moveVector.x, 0, moveVector.y);
                transform.position = newPosition;
            }
            var inputForward = InputActions.Player.Look.ReadValue<Vector2>();
            if (inputForward.magnitude > 0f)
            {
                transform.forward = new Vector3(inputForward.x, 0, inputForward.y);
            }
            else
            {
                var inputPointPosition = InputActions.Player.PointPosition.ReadValue<Vector2>();
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
        if (isLocalPlayer)
        {
            InputActions.Player.Disable();
            InputActions.Dispose();
        }
    }
}
