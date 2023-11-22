using System.Collections.Generic;
using UnityEngine;
public interface IAttack
{
    public int Damage { get; }
    public Vector3 KnockbackDistance { get; }
    public float KnockbackDuration { get; }
    public Vector3 KnockoffInitialVelocity { get; }
    public float KnockoffDuration { get; }
    public List<Buff> Buffs { get; }
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/AttackScriptableObject", order = 1)]
#endif
public class AttackScriptableObject : ScriptableObject, IAttack
{
    public bool ForMyself;
    public LayerMask TargetLayer;
    public AttackRangeScriptableObject AttackRange;
    public int Damage;
    public Vector3 KnockbackDistance;
    public float KnockbackDuration;

    public Vector3 KnockoffInitialVelocity;
    public float KnockoffDuration;

    [XLSXBuffList]
    public List<Buff> Buffs;

    int IAttack.Damage => Damage;

    Vector3 IAttack.KnockbackDistance => KnockbackDistance;

    float IAttack.KnockbackDuration => KnockbackDuration;

    Vector3 IAttack.KnockoffInitialVelocity => KnockoffInitialVelocity;

    float IAttack.KnockoffDuration => KnockoffDuration;

    List<Buff> IAttack.Buffs => Buffs;
}
