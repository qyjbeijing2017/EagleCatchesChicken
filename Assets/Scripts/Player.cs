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


    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
            Forward.Value = transform.forward;

            InputActions = new PlayerInputAction();
            InputActions.Player.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            // var inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            var inputAxis = InputActions.Player.Move.ReadValue<Vector2>();
            if (inputAxis.magnitude > 0f)
            {
                var moveSpeed = BaseMoveSpeed * Time.deltaTime;
                var moveVector = inputAxis * moveSpeed;
                var newPosition = transform.position + new Vector3(moveVector.x, 0, moveVector.y);
                Position.Value = newPosition;
                Forward.Value = new Vector3(moveVector.x, 0, moveVector.y);
            }

        }
        transform.position = Position.Value;
        transform.forward = Forward.Value;
    }

    void OnDestory()
    {
        if (IsOwner)
        {
            InputActions.Dispose();
        }
    }
}
