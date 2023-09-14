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
        if (isLocalPlayer || isServer)
        {
            PlayerRigidbody = GetComponent<Rigidbody>();
            PlayerMove = GetComponent<Move>();
            PlayerJumpManager = GetComponentInChildren<JumpManager>();
            if (PlayerJumpManager == null)
            {
                Debug.LogError("PlayerJumpManager is null");
            }
            PlayerJumpManager.onGrounded += () =>
            {
                if (isServerOnly)
                    animator.SetTrigger("Grounded");
                RpcTrigger("Grounded");
            };
        }
        if (isServer)
        {
            PlayerSkillManager = GetComponent<SkillManager>();
            PlayerSkillManager.OnSkillStart += (SkillIdentity skillIdentity) =>
            {
                if (isServerOnly)
                    animator.SetTrigger(skillIdentity.id);
                RpcTrigger(skillIdentity.id);
            };
            if (PlayerSkillManager.attack != null)
                PlayerSkillManager.attack.onAttack += () =>
                {
                    if (isServerOnly)
                        animator.SetTrigger("Attack");
                    RpcAttack();
                };
        }
    }

    [ClientRpc]
    void RpcTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    [ClientRpc]
    void RpcAttack()
    {
        animator.SetTrigger("Attack");
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
            animator.SetInteger("JumpCount", PlayerMove.jumpCount);
            if(PlayerSkillManager.attack != null)
                animator.SetBool("ReadyAttack", PlayerSkillManager.attack.IsReady);
        }
    }

}
