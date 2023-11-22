using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NetworkAnimator))]
public class PlayerAnimator : PlayerComponent
{
    Animator m_Animator;
    PlayerMove m_PlayerMove;
    PlayerSkill m_PlayerSkill;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_PlayerMove = GetComponent<PlayerMove>();
        m_PlayerSkill = GetComponent<PlayerSkill>();

        m_PlayerMove.onJump += () => m_Animator.SetTrigger("Jump");
        m_PlayerMove.onGrounded += ()=> m_Animator.SetTrigger("Grounded");
        m_PlayerSkill.OnSkill += (index) => m_Animator.SetTrigger($"Skill{index + 1}");
    }

    // Update is called once per frame
    void Update()
    {
        // Move
        var localVelocity = transform.InverseTransformDirection(m_PlayerMove.inputVelocity);
        if (isLocalPlayer)
        {
            m_Animator.SetFloat("Forward", localVelocity.z);
            m_Animator.SetFloat("Right", localVelocity.x);
            m_Animator.SetBool("IsGrounded", m_PlayerMove.isGrounded);
            if (localVelocity.y > 0) m_Animator.SetTrigger("Jump");
            for (var i = 0; i < m_PlayerSkill.RunningSkills.Count; i++)
            {
                if (m_PlayerSkill.RunningSkills[i])
                    m_Animator.SetBool($"Skill{i + 1}", true);
                else
                    m_Animator.SetBool($"Skill{i + 1}", false);
            }
        }
    }
}
