using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Collections.Generic;
#endif

class XLSXBuffList : IXLSXFiledAttribute
{
    public override HashSet<Type> referenceTypes => new HashSet<Type>() { typeof(Buff) };
    public XLSXBuffList()
    {
#if UNITY_EDITOR
        writer = (object obj) =>
        {
            var buffs = obj as List<Buff>;
            var str = "";
            foreach (var buff in buffs)
            {
                if (str != "")
                {
                    str += "|";
                }
                str += buff.name;
            }
            return str;
        };

        reader = (string str) =>
        {
            List<Buff> buffs = new List<Buff>();
            var buffNames = str.Split('|');
            if (buffNames.Length == 1 && buffNames[0] == "")
            {
                return buffs;
            }
            foreach (var buffName in buffNames)
            {
                var buffPath = $"Assets/Prefabs/Buffs/{buffName}.asset";
                var configPath = $"Assets/Configurations/Buff_{buffName}.asset";
                var buffObj = AssetDatabase.LoadAssetAtPath<GameObject>(buffPath);
                var configObj = AssetDatabase.LoadAssetAtPath<BuffScriptableObject>(configPath);

                if (buffObj == null)
                {
                    buffObj = new GameObject(buffName);
                    buffObj.AddComponent<Buff>();
                }
                if (buffObj.GetComponent<Buff>() == null)
                {
                    buffObj.AddComponent<Buff>();
                }
                
                buffObj.GetComponent<Buff>().buffConfig = configObj;
                EditorUtility.SetDirty(buffObj);
                buffs.Add(buffObj.GetComponent<Buff>());
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return buffs;
        };
#endif
    }
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/BuffScriptableObject", order = 1)]
#endif
public class BuffScriptableObject : ScriptableObject
{
    public float Duration;

    public float SpeedMultiplier;
    public float SpeedAddition;
    public float DamageMultiplier;
    public int DamageAddition;
    public float BeHurtMultiplier;
    public int BeHurtAddition;
    public bool BeStunning;


    public float Interval;
    public int BeDamageOverTime;
    public Vector3 KnockbackDistance;
    public float KnockbackDuration;
    public Vector3 KnockoffInitialVelocity;
    public float KnockoffDuration;
}
