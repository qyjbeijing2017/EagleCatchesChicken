using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/AttackScriptableObject", order = 1)]
#endif
public class AttackScriptableObject : ScriptableObject
{
    public bool ForMyself;
    public AttackRangeScriptableObject AttackRange;
    public float Damage;
    public List<Buff> Buffs;
}
