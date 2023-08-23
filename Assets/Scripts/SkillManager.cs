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

    PlayerInputAction InputActions;

    // Start is called before the first frame update
    void Awake()
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
        if(Skills.Count > 0)
            Skills[0].exec(PlayerDamageType);
    }

    void HandleSkill2(InputAction.CallbackContext context)
    {
        if (Skills.Count > 1)
            Skills[1].exec(PlayerDamageType);
    }

    void HandleSkill3(InputAction.CallbackContext context)
    {
        if (Skills.Count > 2)
            Skills[2].exec(PlayerDamageType);
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
