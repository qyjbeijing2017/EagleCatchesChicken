using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class SkillManager : NetworkBehaviour
{
    public DamageType PlayerDamageType;

    [SerializeField]
    List<Skill> Skills = new List<Skill>();

    Skill currentSkill = null;
    int currentSkillIndex = 0;

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
            currentSkill = Skills[0];
        }
    }

    void HandleSkill2(InputAction.CallbackContext context)
    {
        if (Skills.Count > 1)
        {
            Skills[1].exec();
            currentSkill = Skills[1];
        }
    }

    void HandleSkill3(InputAction.CallbackContext context)
    {
        if (Skills.Count > 2)
        {
            Skills[2].exec();
            currentSkill = Skills[2];
        }
    }

    void OnSkillEnd()
    {
        if (currentSkill != null)
        {
            currentSkill.Stop();
            currentSkill = null;
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
