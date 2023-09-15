using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.ComponentModel;

[System.Serializable]
public struct SkillIdentity
{
    [HideInInspector]
    public string name;
    public string id;
    public Skill skill;
}

public class SkillManager : NetworkBehaviour
{


    [SerializeField]
    public Attack Attack;

    [SerializeField]
    public List<SkillIdentity> Skills = new List<SkillIdentity>();

    [Header("Debug")]
    [ReadOnly(true)]
    public DamageType PlayerDamageType = DamageType.None;

    PlayerInputAction InputActions;

    Player MyPlayer;

    BuffManager PlayerBuffManager;

    public Attack attack
    {
        get
        {
            return Attack;
        }
    }

    public bool canMove
    {
        get
        {
            foreach (var skillIdentity in Skills)
            {
                if (
                    skillIdentity.skill != null &&
                    skillIdentity.skill.isRunning &&
                    !skillIdentity.skill.moveInRunning
                )
                {
                    return false;
                }
            }
            if (
                Attack != null &&
                Attack.isRunning &&
                !Attack.moveInRunning
                )
            {
                return false;
            }
            return true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MyPlayer = GetComponent<Player>();
        PlayerBuffManager = GetComponent<BuffManager>();
        InputActions = new PlayerInputAction();
        if (isLocalPlayer)
        {
            InputActions.Skill.Enable();
            var actions = InputActions.Skill.Get().actions;
            for (int i = 0; i < actions.Count; i++)
            {
                var index = i;
                actions[i].performed += (context) => SkillStart(index);
            }
        }
    }

    public event System.Action<SkillIdentity> OnSkillStart;

    [Command]
    void SkillStart(int skillNo)
    {
        if (PlayerBuffManager.isStagger) return;

        var inputCount = InputActions.Skill.Get().actions.Count;

        var skillIdentity = Skills[inputCount * (int)PlayerDamageType + skillNo];
        if (!skillIdentity.skill) return;

        if (skillIdentity.skill.Exec(MyPlayer.PlayerId))
        {
            OnSkillStart?.Invoke(skillIdentity);
        }
    }

    [Server]
    void OnSkillEnd(string skillId)
    {
        if (skillId == "Attack")
        {
            Attack.Stop();
            return;
        }
        var skillIdentities = Skills.FindAll(skillIdentity => skillIdentity.id == skillId);
        foreach (var skill in skillIdentities)
        {
            if (skill.skill)
                skill.skill.Stop();
        }
    }

    // magic is name:damageNo, like Fire:0
    [Server]
    void OnDamage(string magic)
    {
        var magicSplit = magic.Split(':');
        if (magicSplit.Length != 2)
        {
            Debug.LogError($"magicSplit.Length != 2, magic = {magic}");
            return;
        }
        var name = magicSplit[0];
        var damageNo = int.Parse(magicSplit[1]);
        if (name == "Attack")
        {
            Attack.OnDamage(damageNo);
            return;
        }

        var skillIdentities = Skills.FindAll(skillIdentity => skillIdentity.id == name);
        foreach (var skill in skillIdentities)
        {
            if (skill.skill)
                skill.skill.OnDamage(damageNo);
        }
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
