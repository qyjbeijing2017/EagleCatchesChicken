using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerBuff : PlayerComponent
{
    public readonly SyncList<Buff> Buffs = new SyncList<Buff>();
    public float speedMultiplier
    {
        get
        {
            float result = 1;
            foreach (Buff buff in Buffs)
            {
                result *= buff.buffAttribute.SpeedMultiplier;
            }
            return result;
        }
    }
    public float speedAddition
    {
        get
        {
            float result = 0;
            foreach (Buff buff in Buffs)
            {
                result += buff.buffAttribute.SpeedAddition;
            }
            return result;
        }
    }
    public float damageMultiplier
    {
        get
        {
            float result = 1;
            foreach (Buff buff in Buffs)
            {
                result *= buff.buffAttribute.DamageMultiplier;
            }
            return result;
        }
    }
    public float damageAddition
    {
        get
        {
            float result = 0;
            foreach (Buff buff in Buffs)
            {
                result += buff.buffAttribute.DamageAddition;
            }
            return result;
        }
    }
    public float beHurtMultiplier
    {
        get
        {
            float result = 1;
            foreach (Buff buff in Buffs)
            {
                result *= buff.buffAttribute.BeHurtMultiplier;
            }
            return result;
        }
    }
    public float beHurtAddition
    {
        get
        {
            float result = 0;
            foreach (Buff buff in Buffs)
            {
                result += buff.buffAttribute.BeHurtAddition;
            }
            return result;
        }
    }
    public bool beStunning
    {
        get
        {
            foreach (Buff buff in Buffs)
            {
                if (buff.buffAttribute.BeStunning)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
