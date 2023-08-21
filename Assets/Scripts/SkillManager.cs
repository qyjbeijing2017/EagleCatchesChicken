using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class SkillManager : NetworkBehaviour
{
    [SerializeField]
    List<Skill> Skills = new List<Skill>();

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
    }

    void HandleSkill2(InputAction.CallbackContext context)
    {
    }

    void HandleSkill3(InputAction.CallbackContext context)
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
