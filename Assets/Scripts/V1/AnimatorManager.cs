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
    // Rigidbody PlayerRigidbody;
    Move PlayerMove;

    SkillManager PlayerSkillManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerSkillManager = GetComponent<SkillManager>();


        if (isLocalPlayer || isServer)
        {
            // PlayerRigidbody = GetComponent<Rigidbody>();
            PlayerMove = GetComponent<Move>();
        }

        if (isServer)
        {
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
            var forwardVelocity = Vector3.Dot(PlayerMove.moveVelocity, transform.forward) / PlayerMove.baseMoveSpeed;
            var rightVelocity = Vector3.Dot(PlayerMove.moveVelocity, transform.right) / PlayerMove.baseMoveSpeed;
            var upVelocity = PlayerMove.moveVelocity.y;

            animator.SetFloat("Forward", forwardVelocity);
            animator.SetFloat("Right", rightVelocity);
            animator.SetInteger("JumpCount", PlayerMove.jumpCount);
            animator.SetBool("Grounded", PlayerMove.isGrounded);
            if(PlayerSkillManager.attack != null)
                animator.SetBool("ReadyAttack", PlayerSkillManager.attack.IsReady);
        }
    }

}
