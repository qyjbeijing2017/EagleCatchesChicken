using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using Unity.VisualScripting;

[CustomEditor(typeof(SkillManager))]
public class SkillManagerEditor : Editor
{
    void Awake()
    {
        var skillManager = (SkillManager)target;
        var InputActions = new PlayerInputAction();
        var actions = InputActions.Skill.Get().actions;
        var damageTypeNames = System.Enum.GetNames(typeof(DamageType));
        var newList = new List<SkillIdentity>(actions.Count * damageTypeNames.Length);
        var oldList = skillManager.Skills;
        foreach (var damageTypeName in damageTypeNames)
        {
            foreach (var action in actions)
            {

                var name = $"{damageTypeName}\t{action.name}";
                var skillIdentity = oldList.Find((SkillIdentity skillIdentity) => skillIdentity.name == name);
                if (skillIdentity.name == null)
                {
                    skillIdentity.name = name;
                    skillIdentity.id = $"{action.name}";
                }
                newList.Add(skillIdentity);
            }
        }

        newList.Sort((SkillIdentity a, SkillIdentity b) =>
        {
            var oldIndexA = oldList.FindIndex((SkillIdentity skillIdentity) => skillIdentity.name == a.name);
            var oldIndexB = oldList.FindIndex((SkillIdentity skillIdentity) => skillIdentity.name == b.name);
            if (oldIndexA == -1 && oldIndexB == -1)
            {
                return a.name.CompareTo(b.name);
            }
            return oldIndexA - oldIndexB;
        });
        skillManager.Skills = newList;
    }
}
