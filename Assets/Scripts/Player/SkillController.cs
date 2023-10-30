using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    Animator m_Animator;

    bool m_SkillRunning = false;

    public bool skillRunning
    {
        get
        {
            return m_SkillRunning;
        }
    }

    PlayerInputAction InputActions;


    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        InputActions = new PlayerInputAction();
        InputActions.Skill.Enable();
        var actions = InputActions.Skill.Get().actions;
        for (int i = 0; i < actions.Count; i++)
        {
            var index = i;
            actions[i].performed += (context) => SkillStart(index);
        }
    }


    void SkillStart(int skillNo)
    {
        if (m_SkillRunning) return;
        m_Animator.SetTrigger("Skill" + (skillNo + 1));
        m_SkillRunning = true;
    }

    public void Shoot()
    {
    }


    public void SkillEnd()
    {
        m_SkillRunning = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
