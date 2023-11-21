using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/BuffScriptableObject", order = 1)]
#endif
public class BuffScriptableObject  : ScriptableObject
{
    public float Duration;
    public float Interval;
    public float SpeedMultiplier;
    public float SpeedAddition;
    public float DamageMultiplier;
    public float DamageAddition;
    public float BeHurtMultiplier;
    public float BeHurtAddition;
    public float BeDamageOverTime;
    public bool BeStunning;
}
