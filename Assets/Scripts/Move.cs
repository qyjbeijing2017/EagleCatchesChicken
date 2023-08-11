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
    // Start is called before the first frame update


    Rigidbody PlayerRigidbody;
    PlayerInputAction InputActions;
    BuffManager PlayerBuffManager;
    void Start()
    {
        if(isLocalPlayer) {
            InputActions = new PlayerInputAction();
            InputActions.Player.Enable();
            InputActions.Player.Jump.performed += OnJump;

            PlayerRigidbody = GetComponent<Rigidbody>();
            PlayerBuffManager = GetComponent<BuffManager>();
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
            
            var inputAxis = InputActions.Player.Move.ReadValue<Vector2>();
            if (inputAxis.magnitude > 0f)
            {
                var moveSpeed = (BaseMoveSpeed - PlayerBuffManager.slowDownSpeed) * Time.deltaTime * (1 - PlayerBuffManager.slowDownPer);
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
        if (isLocalPlayer && InputActions != null)
        {
            Debug.Log(InputActions);
            InputActions.Player.Disable();
            InputActions.Dispose();
        }
    }
}
