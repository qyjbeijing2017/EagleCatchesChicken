using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Collections.Generic;
#endif

class XLSXBuffList : IXLSXFiledAttribute
{
    public XLSXBuffList()
    {
#if UNITY_EDITOR
        writer = (object obj) =>
        {
            var buffs = obj as List<GameObject>;
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
            List<GameObject> buffs = new List<GameObject>();
            var buffNames = str.Split('|');
            if (buffNames.Length == 1 && buffNames[0] == "")
            {
                return buffs;
            }
            foreach (var buffName in buffNames)
            {
                var buffPath = $"Assets/Prefabs/Buffs/{buffName}.prefab";
                var configPath = $"Assets/Configurations/Buff_{buffName}.asset";
                var buffObj = AssetDatabase.LoadAssetAtPath<GameObject>(buffPath);
                var configObj = AssetDatabase.LoadAssetAtPath<BuffScriptableObject>(configPath);

                if (buffObj == null)
                {
                    var gameObject = new GameObject(buffName);
                    gameObject.AddComponent<Buff>();
                    buffObj = PrefabUtility.SaveAsPrefabAsset(gameObject, buffPath);
                    GameObject.DestroyImmediate(gameObject);
                }
                if (buffObj.GetComponent<Buff>() == null)
                {
                    buffObj.AddComponent<Buff>();
                }
                
                buffObj.GetComponent<Buff>().buffConfig = configObj;
                EditorUtility.SetDirty(buffObj);
                buffs.Add(buffObj);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return buffs;
        };
#endif
    }
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Buff_", menuName = "ScriptableObjects/BuffScriptableObject", order = 1)]
#endif
[XLSXLocal]
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
