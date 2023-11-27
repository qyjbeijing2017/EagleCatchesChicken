using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

[Serializable]
public struct AttackEvent
{
    public float time;
    public AttackScriptableObject attack;
}

class XLSXAttackEventList : IXLSXFiledAttribute
{
    public override HashSet<Type> referenceTypes => new HashSet<Type>() { typeof(AttackScriptableObject) };
    public XLSXAttackEventList()
    {
#if UNITY_EDITOR
        writer = (object obj) =>
        {
            var attackEvents = obj as List<AttackEvent>;
            var str = "";
            foreach (var attackEvent in attackEvents)
            {
                if (str != "")
                {
                    str += "\n";
                }
                str += $"[{attackEvent.time}]{attackEvent.attack.name.Replace("Attack_", "")}";
            }
            return str;
        };

        reader = (string str) =>
        {
            List<AttackEvent> attackEvents = new List<AttackEvent>();
            var attackEventStrings = str.Split('\n');
            if (attackEventStrings.Length == 1 && attackEventStrings[0] == "")
            {
                return attackEvents;
            }

            foreach (var attackEventStr in attackEventStrings)
            {
                var attackEvent = new AttackEvent();
                var attackEventStrSplit = attackEventStr.Split(']');
                attackEvent.time = float.Parse(attackEventStrSplit[0].Substring(1));
                var configPath = $"Assets/Configurations/Attack_{attackEventStrSplit[1]}.asset";
                attackEvent.attack = AssetDatabase.LoadAssetAtPath<AttackScriptableObject>(configPath);
                attackEvents.Add(attackEvent);
            }

            attackEvents.Sort((a, b) => a.time.CompareTo(b.time));

            return attackEvents;
        };
#endif
    }
}



#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Skill_", menuName = "ScriptableObjects/SkillScriptableObject", order = 1)]
#endif
[XLSXLocal]
public class SkillScriptableObject : ScriptableObject
{
    public bool AutoAttack;
    public AttackRangeScriptableObject AutoPrepareRange;
    public AttackRangeScriptableObject AutoAttackRange;
    public LayerMask AutoTargetLayer = 1 << 8 | 1 << 9 | 1 << 10;
    public bool ForceMove = true;
    public float Duration;
    public float CoolDown;

    [XLSXWriteOnly]
    public AnimationCurve DashSpeed;
    [XLSXWriteOnly]
    public AnimationCurve DashRotationY;
    [XLSXAttackEventList]
    public List<AttackEvent> AttackEvents;
}
