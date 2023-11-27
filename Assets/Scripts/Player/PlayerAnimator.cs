using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NetworkAnimator))]
public class PlayerAnimator : PlayerComponent
{
    Animator m_Animator;
    PlayerMove m_PlayerMove;
    PlayerSkill m_PlayerSkill;
    PlayerHealth m_PlayerHealth;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_PlayerMove = GetComponent<PlayerMove>();
        m_PlayerSkill = GetComponent<PlayerSkill>();
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_PlayerMove.onJump += () => {
            m_Animator.SetTrigger("OnJump");
        };
        m_PlayerSkill.OnSkill += (index) => m_Animator.SetTrigger($"OnSkill{index}");
    }

    // Update is called once per frame
    void Update()
    {
        // Move
        var localVelocity = transform.worldToLocalMatrix * m_PlayerMove.inputVelocity / playerConfig.MoveSpeed;
        if (isLocalPlayer || isOwned || isServer && identity == PlayerIdentity.Dummy)
        {
            if (m_PlayerHealth.isKnockedBack || m_PlayerHealth.isKnockedOff)
            {
                m_Animator.speed = 0;
            }
            else
            {
                m_Animator.speed = 1;
            }
            m_Animator.SetFloat("Forward", localVelocity.z);
            m_Animator.SetFloat("Right", localVelocity.x);
            m_Animator.SetBool("IsGrounded", m_PlayerMove.isGrounded);
            for (var i = 0; i < m_PlayerSkill.RunningSkills.Count; i++)
            {
                if (m_PlayerSkill.RunningSkills[i])
                    m_Animator.SetBool($"Skill{i}", true);
                else
                    m_Animator.SetBool($"Skill{i}", false);
            }
        }
    }
}
