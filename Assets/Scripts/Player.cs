using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    private NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> Forward = new NetworkVariable<Vector3>();

    [SerializeField]
    private float BaseMoveSpeed = 1f;


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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            var inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            var moveSpeed = BaseMoveSpeed * Time.deltaTime;
            var moveVector = inputAxis * moveSpeed;
            var newPosition = transform.position + moveVector;
            Position.Value = newPosition;
            Forward.Value = moveVector.normalized;
        }
        transform.position = Position.Value;
        transform.forward = Forward.Value;
    }
}
