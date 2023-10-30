using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveCharactor : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Base move speed in meters per second")]
    float BaseMoveSpeed = 10f;

    PlayerInputActionV1 InputActions;
    CharacterController CharacterController;
    float GravitationAcceleration = 9.8f;



    // Start is called before the first frame update
    void Start()
    {
        InputActions = new PlayerInputActionV1();
        InputActions.Move.Enable();
        CharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var inputAxis = InputActions.Move.Move.ReadValue<Vector2>();
        var moveVelocity = new Vector3(inputAxis.x, 0, inputAxis.y) * BaseMoveSpeed;
        moveVelocity.y -= GravitationAcceleration;
        CharacterController.Move(moveVelocity * Time.deltaTime);
    }
}
