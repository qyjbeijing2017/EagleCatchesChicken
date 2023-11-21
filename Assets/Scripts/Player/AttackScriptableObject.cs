using System.Collections.Generic;
using UnityEngine;

public interface Attack
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
public class AttackScriptableObject : ScriptableObject, Attack
{
    public bool ForMyself;
    public AttackRangeScriptableObject AttackRange;
    public int Damage;
    public Vector3 KnockbackDistance;
    public float KnockbackDuration;

    public Vector3 KnockoffInitialVelocity;
    public float KnockoffDuration;

    [XLSXBuffList]
    public List<Buff> Buffs;

    int Attack.Damage => Damage;

    Vector3 Attack.KnockbackDistance => KnockbackDistance;

    float Attack.KnockbackDuration => KnockbackDuration;

    Vector3 Attack.KnockoffInitialVelocity => KnockoffInitialVelocity;

    float Attack.KnockoffDuration => KnockoffDuration;

    List<Buff> Attack.Buffs => Buffs;
}
