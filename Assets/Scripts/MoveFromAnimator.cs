using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoveFromAnimator : NetworkBehaviour
{
    Animator animator;
    Rigidbody PlayerRigidbody;
    Move PlayerMove;
    JumpManager PlayerJumpManager;

    void Start()
    {
        if(isLocalPlayer) {
            animator = GetComponent<Animator>();
            PlayerRigidbody = GetComponent<Rigidbody>();
            PlayerMove = GetComponent<Move>();
            PlayerJumpManager = GetComponentInChildren<JumpManager>();

        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            var velocity = PlayerMove.moveVelocity;

            var forwardDir = new Vector2(transform.forward.x, transform.forward.z);
            var rightDir = new Vector2(transform.right.x, transform.right.z);

            var forwardVelocity = Vector2.Dot(velocity, forwardDir);
            var rightVelocity = Vector2.Dot(velocity, rightDir);

            animator.SetFloat("Forward", forwardVelocity);
            animator.SetFloat("Right", rightVelocity);
            animator.SetFloat("Up", PlayerRigidbody.velocity.y);
            animator.SetFloat("Height", PlayerJumpManager.height);
            animator.SetInteger("JumpCount", PlayerMove.jumpCount);
        }
    }

}
