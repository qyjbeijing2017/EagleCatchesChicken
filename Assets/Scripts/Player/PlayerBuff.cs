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
                result *= buff.buffConfig.SpeedMultiplier;
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
                result += buff.buffConfig.SpeedAddition;
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
                result *= buff.buffConfig.DamageMultiplier;
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
                result += buff.buffConfig.DamageAddition;
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
                result *= buff.buffConfig.BeHurtMultiplier;
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
                result += buff.buffConfig.BeHurtAddition;
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
                if (buff.buffConfig.BeStunning)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
