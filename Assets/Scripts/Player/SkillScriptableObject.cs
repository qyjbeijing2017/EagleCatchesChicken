using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/SkillScriptableObject", order = 1)]
#endif
public class SkillScriptableObject : ScriptableObject
{
    public bool Moveable;
    public bool Duration;
    [XLSXWriteOnly]
    public AnimationCurve Speed;
    [XLSXWriteOnly]
    public AnimationCurve RotationY;
    public List<AttackScriptableObject> Attack;
}
