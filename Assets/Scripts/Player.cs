using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    private NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> Forward = new NetworkVariable<Vector3>();

    [SerializeField]
    [Tooltip("Base move speed in meters per second")]
    private float BaseMoveSpeed = 1f;


    private PlayerInputAction InputActions;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            InputActions = new PlayerInputAction();
            InputActions.Player.Enable();
            InputActions.Player.Jump.performed += OnJump;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        this.GetComponent<Rigidbody>().velocity = Vector3.up * 5f;
    }

    // [ServerRpc]
    // void SubmitPositionRequestServerRpc(Vector3 position, Vector3 forward)
    // {
    //     Position.Value = position;
    //     Forward.Value = forward;
    // }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            var inputAxis = InputActions.Player.Move.ReadValue<Vector2>();
            if (inputAxis.magnitude > 0f)
            {
                var moveSpeed = BaseMoveSpeed * Time.deltaTime;
                var moveVector = inputAxis * moveSpeed;
                var newPosition = transform.position + new Vector3(moveVector.x, 0, moveVector.y);
                transform.position = newPosition;
                transform.forward = new Vector3(moveVector.x, 0, moveVector.y);
            }
        }
    }

    public override void OnDestroy()
    {
        if (IsOwner)
        {
            InputActions.Player.Disable();
            InputActions.Dispose();
        }
        base.OnDestroy();
    }
}
