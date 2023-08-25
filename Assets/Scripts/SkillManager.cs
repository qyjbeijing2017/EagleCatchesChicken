using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SkillManager : NetworkBehaviour
{
    public DamageType PlayerDamageType;

    [SerializeField]
    List<Skill> Skills = new List<Skill>();

    Skill CurrentSkill = null;
    int CurrentSkillIndex = -1;

    public bool isSkillRunning
    {
        get
        {
            return CurrentSkill != null;
        }
    }

    public int skillNo
    {
        get
        {
            return CurrentSkillIndex;
        }
    }

    PlayerInputAction InputActions;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            InputActions = new PlayerInputAction();
            InputActions.Skill.Enable();

            InputActions.Skill.Skill1.performed += HandleSkill1;
            InputActions.Skill.Skill2.performed += HandleSkill2;
            InputActions.Skill.Skill3.performed += HandleSkill3;
        }
    }

    void HandleSkill1(InputAction.CallbackContext context)
    {
        if (Skills.Count > 0)
        {
            Skills[0].exec();
            CurrentSkill = Skills[0];
            CurrentSkillIndex = 0;
        }
    }

    void HandleSkill2(InputAction.CallbackContext context)
    {
        if (Skills.Count > 1)
        {
            Skills[1].exec();
            CurrentSkill = Skills[1];
            CurrentSkillIndex = 1;
        }
    }

    void HandleSkill3(InputAction.CallbackContext context)
    {
        if (Skills.Count > 2)
        {
            Skills[2].exec();
            CurrentSkill = Skills[2];
            CurrentSkillIndex = 2;
        }
    }

    void OnSkillEnd()
    {
        if (CurrentSkill != null)
        {
            CurrentSkill.Stop();
            CurrentSkill = null;
            CurrentSkillIndex = -1;
        }
    }

    void OnDamage(int damageIndex)
    {
        if (CurrentSkill != null)
        {
            CurrentSkill.OnDamage(damageIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (isLocalPlayer && InputActions != null)
        {
            InputActions.Move.Disable();
            InputActions.Dispose();
        }
    }
}
