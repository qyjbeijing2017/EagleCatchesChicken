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
    Attack Attack;

    [SerializeField]
    List<Skill> Skills = new List<Skill>();

    PlayerInputAction InputActions;

    Player MyPlayer;

    BuffManager PlayerBuffManager;

    public bool canMove{
        get{
            foreach(var skill in Skills) {
                if(!skill.moveInRunning && skill.isRunning) {
                    return false;
                }
            }
            return true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MyPlayer = GetComponent<Player>();
        PlayerBuffManager = GetComponent<BuffManager>();
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
            SkillStart(0);
        }
    }

    void HandleSkill2(InputAction.CallbackContext context)
    {
        if (Skills.Count > 1)
        {
            SkillStart(1);
        }
    }

    void HandleSkill3(InputAction.CallbackContext context)
    {
        if (Skills.Count > 2)
        {
            SkillStart(2);
        }
    }

    public event System.Action<int> OnSkillStart;

    [Command]
    void SkillStart(int skillNo)
    {
        if(PlayerBuffManager.isStagger) return;
        if(skillNo >= 0 && skillNo < Skills.Count)
        {
            if(Skills[skillNo].exec(MyPlayer.PlayerId)){
                OnSkillStart?.Invoke(skillNo);
            }
        }
    }

    [Server]
    void OnSkillEnd(string skillName)
    {
        Skills.FindAll(skill => skill.name == skillName).ForEach(skill => skill.Stop());
    }

    // magic is name:damageNo, like Fire:0
    [Server]
    void OnDamage(string magic)
    {
        var magicSplit = magic.Split(':');
        if(magicSplit.Length != 2) {
            Debug.LogError($"magicSplit.Length != 2, magic = {magic}");
            return;
        }
        var name = magicSplit[0];
        var damageNo = int.Parse(magicSplit[1]);
        Skills.FindAll(skill => skill.name == name).ForEach(skill => skill.OnDamage(damageNo));
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
