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
[CreateAssetMenu(fileName = "Attack_", menuName = "ScriptableObjects/AttackScriptableObject", order = 1)]
#endif
[XLSXLocal]
public class AttackScriptableObject : ScriptableObject, IAttack
{
    public bool ForMyself = false;
    public LayerMask TargetLayer = 1 << 8 | 1 << 9;
    public AttackRangeScriptableObject AttackRange = null;
    public int Damage = 0;
    public Vector3 KnockbackDistance = Vector3.zero;
    public float KnockbackDuration = 0f;

    public Vector3 KnockoffInitialVelocity = Vector3.zero;
    public float KnockoffDuration = 0f;

    [XLSXBullet]
    public GameObject ShootBullet = null;

    [XLSXBuffList]
    public List<GameObject> Buffs = new List<GameObject>();

    int IAttack.Damage => Damage;

    Vector3 IAttack.KnockbackDistance => KnockbackDistance;

    float IAttack.KnockbackDuration => KnockbackDuration;

    Vector3 IAttack.KnockoffInitialVelocity => KnockoffInitialVelocity;

    float IAttack.KnockoffDuration => KnockoffDuration;

    List<Buff> IAttack.Buffs
    {
        get
        {
            List<Buff> buffs = new List<Buff>();
            foreach (var buff in Buffs)
            {
                buffs.Add(buff.GetComponent<Buff>());
            }
            return buffs;

        }
    }
}
