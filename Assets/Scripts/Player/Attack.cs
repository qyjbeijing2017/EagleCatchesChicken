using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Attack : IAttack
{
    public int Damage { get; set; }

    public Vector3 KnockbackDistance { get; set; }

    public float KnockbackDuration { get; set; }

    public Vector3 KnockoffInitialVelocity { get; set; }

    public float KnockoffDuration { get; set; }

    public List<Buff> Buffs { get; set; }
}
