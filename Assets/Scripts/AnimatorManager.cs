using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NetworkAnimator))]
public class AnimatorManager : NetworkBehaviour
{
    Animator animator;
    Rigidbody PlayerRigidbody;
    Move PlayerMove;
    JumpManager PlayerJumpManager;

    SkillManager PlayerSkillManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        if(isLocalPlayer || isServer) {
            PlayerRigidbody = GetComponent<Rigidbody>();
            PlayerMove = GetComponent<Move>();
            PlayerJumpManager = GetComponentInChildren<JumpManager>();
            if(PlayerJumpManager == null) {
                Debug.LogError("PlayerJumpManager is null");
            }
        }
        if(isServer) {
            PlayerSkillManager = GetComponent<SkillManager>();
            PlayerSkillManager.OnSkillStart += (int skillNo)=>{
                animator.SetTrigger($"Skill{skillNo}");
                RpcSkillStart(skillNo);
            };
        }
    }

    [ClientRpc]
    void RpcSkillStart(int skillNo)
    {
        animator.SetTrigger($"Skill{skillNo}");
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
