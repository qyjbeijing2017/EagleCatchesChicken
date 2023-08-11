using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoveFromAnimator : NetworkBehaviour
{
    Animator animator;
    PlayerInputAction InputActions;

    [SerializeField]
    float CharactorMovePower = 0.0001f;

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

            var inputAxis = InputActions.Player.Move.ReadValue<Vector2>();            
            var localInputAxis = transform.InverseTransformDirection(new Vector3(inputAxis.x, 0, inputAxis.y));
            var currentMoveVector = new Vector3(animator.GetFloat("MoveX"), 0, animator.GetFloat("MoveY"));

            var localMoveVector = Vector3.MoveTowards(currentMoveVector, localInputAxis, Time.deltaTime * CharactorMovePower);
            

            animator.SetFloat("MoveX", localMoveVector.x);
            animator.SetFloat("MoveY", localMoveVector.z);
        }
    }

    void OnDestroy()
    {
        if (isLocalPlayer && InputActions != null)
        {
            InputActions.Player.Disable();
            InputActions.Dispose();
        }
    }

}
