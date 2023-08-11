using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoveFromAnimator : NetworkBehaviour
{
    Animator animator;
    PlayerInputAction InputActions;

    void Start()
    {
        if(isLocalPlayer) {
            InputActions = new PlayerInputAction();
            InputActions.Player.Enable();
            animator = GetComponent<Animator>();
        }
    }

        void Update()
    {
        if (isLocalPlayer)
        {
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

            var moveVector = InputActions.Player.Move.ReadValue<Vector2>();
            animator.SetFloat("MoveX", moveVector.x);
            animator.SetFloat("MoveY", moveVector.y);
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
